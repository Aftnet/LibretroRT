#include "pch.h"
#include "VBAMCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace VBAMRT;
using namespace LibretroRT_Tools;

VBAMCoreInternal^ coreInstance = nullptr;

unsigned int VBAMCoreInternal::SerializationSize::get()
{
	return retro_serialize_size();
}

VBAMCoreInternal^ VBAMCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new VBAMCoreInternal();

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

VBAMCoreInternal::VBAMCoreInternal()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);
}

VBAMCoreInternal::~VBAMCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

bool VBAMCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
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

bool VBAMCoreInternal::LoadGameInternal(IStorageFile^ gameFile)
{
	std::vector<unsigned char> gameData;
	ReadFileToMemory(gameData, gameFile);

	auto gameInfo = GenerateGameInfo(gameData);
	if (!retro_load_game(&gameInfo))
	{
		return false;
	}

	retro_system_av_info info;
	retro_get_system_av_info(&info);
	SetAVInfo(info);

	return true;
}

void VBAMCoreInternal::UnloadGameInternal()
{
	retro_unload_game();
}

void VBAMCoreInternal::RunFrameInternal()
{
	retro_run();
}

void VBAMCoreInternal::Reset()
{
	retro_reset();
}

bool VBAMCoreInternal::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool VBAMCoreInternal::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}