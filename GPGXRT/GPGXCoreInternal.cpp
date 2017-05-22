#include "pch.h"
#include "GPGXCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRTSupport/Converter.h"

using namespace GPGXRT;
using namespace LibretroRTSupport;

GPGXCoreInternal^ coreInstance = nullptr;

GPGXCoreInternal^ GPGXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GPGXCoreInternal();
	}
	return coreInstance;
}

GPGXCoreInternal::GPGXCoreInternal()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);

	retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
	retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
	retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
	retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
	retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
	retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });

	retro_init();
}

GPGXCoreInternal::~GPGXCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

void GPGXCoreInternal::LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream)
{
	retro_game_info gameInfo;
	gameInfo.path = "C:\\lol\\somePath.smd";
	retro_load_game(&gameInfo);

	retro_system_av_info info;
	retro_get_system_av_info(&info);
	SetAVInfo(info);
}

void GPGXRT::GPGXCoreInternal::UnloadGame()
{
	retro_unload_game();
}

void GPGXRT::GPGXCoreInternal::RunFrame()
{
	retro_run();
}

void GPGXRT::GPGXCoreInternal::Reset()
{
	retro_reset();
}
