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
		retro_init();
	}

	return coreInstance;
}

MelonDSCoreInternal::MelonDSCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit,
	true, true)
{
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_E.bin", L"Mega-CD (Model 1 1.00 Europe) BIOS", L"e66fa1dc5820d254611fdcdba0662372"));
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_J.bin", L"Mega-CD (Model 1 1.00 Japan) BIOS", L"278a9397d192149e84e820ac621a8edd"));
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_U.bin", L"Mega-CD (Model 1 1.00 USA) BIOS", L"2efd74e3232ff260e371b99f84024f7f"));
}

MelonDSCoreInternal::~MelonDSCoreInternal()
{
	coreInstance = nullptr;
}