#include "pch.h"
#include "GPGXCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace GPGXRT;
using namespace LibretroRT_Tools;

GPGXCoreInternal^ coreInstance = nullptr;

unsigned int GPGXCoreInternal::SerializationSize::get()
{
	return retro_serialize_size();
}

GPGXCoreInternal^ GPGXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GPGXCoreInternal();

		retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
		retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
		retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
		retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
		retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
		retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });

		retro_init();
	}
	return coreInstance;
}

GPGXCoreInternal::GPGXCoreInternal()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);
}

GPGXCoreInternal::~GPGXCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

bool GPGXCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
{
	if (CoreBase::EnvironmentHandler(cmd, data))
		return true;
	
	switch (cmd)
	{
	case RETRO_ENVIRONMENT_GET_VARIABLE:
		auto varptr = (retro_variable*)data;
		varptr->value = "";
		return true;
	}

	return false;
}

bool GPGXCoreInternal::LoadGameInternal(IStorageFile^ gameFile)
{
	static auto gamePathStr = Converter::PlatformToCPPString(gameFile->Path);
	gameStream = concurrency::create_task(gameFile->OpenAsync(FileAccessMode::Read)).get();

	auto gameInfo = GenerateGameInfo(gameFile->Path, gameStream->Size);
	if (!retro_load_game(&gameInfo))
	{
		return false;
	}

	retro_system_av_info info;
	retro_get_system_av_info(&info);
	SetAVInfo(info);

	return true;
}

void GPGXCoreInternal::UnloadGameInternal()
{
	retro_unload_game();
	gameStream = nullptr;
}

void GPGXCoreInternal::RunFrameInternal()
{
	if (gameStream == nullptr)
		return;

	retro_run();
}

void GPGXCoreInternal::Reset()
{
	retro_reset();
}

bool GPGXCoreInternal::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool GPGXCoreInternal::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}