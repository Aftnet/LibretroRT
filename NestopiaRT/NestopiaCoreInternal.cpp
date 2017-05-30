#include "pch.h"
#include "NestopiaCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRTSupport/Converter.h"

using namespace NestopiaRT;
using namespace LibretroRTSupport;

NestopiaCoreInternal^ coreInstance = nullptr;

unsigned int NestopiaCoreInternal::SerializationSize::get()
{
	return retro_serialize_size();
}

NestopiaCoreInternal^ NestopiaCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new NestopiaCoreInternal();

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

NestopiaCoreInternal::NestopiaCoreInternal()
{
	pixelFormat = LibretroRT::PixelFormats::FormatXRGB8888;

	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);
}

NestopiaCoreInternal::~NestopiaCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

bool NestopiaCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
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

bool NestopiaCoreInternal::LoadGame(IStorageFile^ gameFile)
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

void NestopiaCoreInternal::UnloadGame()
{
	retro_unload_game();
}

void NestopiaCoreInternal::RunFrame()
{
	retro_run();
}

void NestopiaCoreInternal::Reset()
{
	retro_reset();
}

bool NestopiaCoreInternal::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool NestopiaCoreInternal::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}