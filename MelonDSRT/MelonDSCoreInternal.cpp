#include "pch.h"
#include "MelonDSCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT/libretro_extra.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace MelonDSRT;
using namespace LibretroRT_Tools;

MelonDSCoreInternal^ coreInstance = nullptr;

MelonDSCoreInternal^ MelonDSCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new MelonDSCoreInternal();

		retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
		retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
		retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
		retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
		retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
		retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });
		retro_extra_set_open_file([](String^ filePath, FileAccessMode accessMode) { return coreInstance->OpenFileStream(filePath, accessMode); });
		retro_extra_set_close_file([](IRandomAccessStream^ stream) { coreInstance->CloseFileStream(stream); });
	}

	return coreInstance;
}

MelonDSCoreInternal::MelonDSCoreInternal() : LibretroRT_Tools::CoreBase(retro_init, retro_deinit,
	retro_get_system_info, retro_get_system_av_info, retro_set_controller_port_device,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize,
	true, true, false)
{
	fileDependencies->Append(ref new FileDependency(L"bios7.bin", L"Nintendo DS ARM7 BIOS", L"df692a80a5b1bc90728bc3dfc76cd948"));
	fileDependencies->Append(ref new FileDependency(L"bios9.bin", L"Nintendo DS ARM9 BIOS", L"a392174eb3e572fed6447e956bde4b25"));
	fileDependencies->Append(ref new FileDependency(L"firmware.bin", L"Nintendo DS Firmware", L"b10f39a8a5a573753406f9da2e7232c8"));
}

MelonDSCoreInternal::~MelonDSCoreInternal()
{
	coreInstance = nullptr;
}