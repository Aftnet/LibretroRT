#include "pch.h"
#include "GPGXCore.h"

#include "../LibretroRT/libretro.h"

using namespace GPGXRT;
using namespace LibretroRTSupport;

GPGXCore^ coreInstance = nullptr;

GPGXCore^ GPGXCore::GetInstance()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GPGXCore();
	}
	return coreInstance;
}

GPGXCore::GPGXCore()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);

	retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
	retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
	retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
	retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
	retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });

	retro_init();
}

GPGXCore::~GPGXCore()
{
	retro_deinit();
	coreInstance = nullptr;
}

void GPGXRT::GPGXCore::LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream)
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::GPGXCore::UnloadGame()
{
	retro_unload_game();
}

void GPGXRT::GPGXCore::RunFrame()
{
	retro_run();
}

void GPGXRT::GPGXCore::Reset()
{
	retro_reset();
}
