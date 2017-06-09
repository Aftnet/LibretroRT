#include "pch.h"
#include "FCEUMMCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace FCEUMMRT;
using namespace LibretroRT_Tools;

FCEUMMCoreInternal^ coreInstance = nullptr;

unsigned int FCEUMMCoreInternal::SerializationSize::get()
{
	return retro_serialize_size();
}

FCEUMMCoreInternal^ FCEUMMCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new FCEUMMCoreInternal();

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

FCEUMMCoreInternal::FCEUMMCoreInternal()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);
}

FCEUMMCoreInternal::~FCEUMMCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

bool FCEUMMCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
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

bool FCEUMMCoreInternal::LoadGame(IStorageFile^ gameFile)
{
	std::vector<unsigned char> gameData;
	ReadFileToMemory(gameData, gameFile);

	auto gameInfo = GenerateGameInfo(gameData);

	static std::string gamePath = Converter::PlatformToCPPString(gameFile->Name);
	gameInfo.path = gamePath.data();
	if (!retro_load_game(&gameInfo))
	{
		return false;
	}

	retro_system_av_info info;
	retro_get_system_av_info(&info);
	SetAVInfo(info);

	return true;
}

void FCEUMMCoreInternal::UnloadGame()
{
	retro_unload_game();
}

void FCEUMMCoreInternal::RunFrame()
{
	retro_run();
}

void FCEUMMCoreInternal::Reset()
{
	retro_reset();
}

bool FCEUMMCoreInternal::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool FCEUMMCoreInternal::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}