#include "pch.h"
#include "BeetleNGPCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT/libretro_extra.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace BeetleNGPRT;
using namespace LibretroRT_Tools;

BeetleNGPCoreInternal^ coreInstance = nullptr;

BeetleNGPCoreInternal^ BeetleNGPCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetleNGPCoreInternal();

		retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
		retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
		retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
		retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
		retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
		retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });
		retro_extra_set_open_file([](String^ filePath, FileAccessMode accessMode) { return coreInstance->OpenFileStream(filePath, accessMode); });
		retro_extra_set_close_file([](IRandomAccessStream^ stream) { coreInstance->CloseFileStream(stream); });
		retro_init();
	}

	return coreInstance;
}

BeetleNGPCoreInternal::BeetleNGPCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit,
	true, true)
{
	fileDependencies->Append(ref new FileDependency(L"scph5500.bin", L"PlayStation (v3.0 09/09/96 J) BIOS", L"8dd7d5296a650fac7319bce665a6a53c"));
	fileDependencies->Append(ref new FileDependency(L"scph5501.bin", L"PlayStation (v3.0 11/18/96 A) BIOS", L"490f666e1afb15b7362b406ed1cea246"));
	fileDependencies->Append(ref new FileDependency(L"scph5502.bin", L"PlayStation (v3.0 01/06/97 E) BIOS", L"32736f17079d0b2b7024407c39bd3050"));
}

BeetleNGPCoreInternal::~BeetleNGPCoreInternal()
{
	coreInstance = nullptr;
}

void BeetleNGPCoreInternal::OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options)
{
	options->Lookup(L"beetle_psx_frame_duping_enable")->SelectedValueIx = 1;
	options->Lookup(L"beetle_psx_analog_calibration")->SelectedValueIx = 1;
	options->Lookup(L"beetle_psx_skipbios")->SelectedValueIx = 1;
}