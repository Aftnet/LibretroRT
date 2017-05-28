#include "pch.h"
#include "Snes9XCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRTSupport/Converter.h"

using namespace Snes9XRT;
using namespace LibretroRTSupport;

Snes9XCoreInternal^ coreInstance = nullptr;

unsigned int Snes9XCoreInternal::SerializationSize::get()
{
	return retro_serialize_size();
}

Snes9XCoreInternal^ Snes9XCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new Snes9XCoreInternal();
	}
	return coreInstance;
}

Snes9XCoreInternal::Snes9XCoreInternal()
{
	pixelFormat = PixelFormats::FormatRGB565;

	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);

	retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
	retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
	retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
	retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
	retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
	retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });
	retro_set_game_read([](void* buffer, size_t requested) { return coreInstance->ReadGameFileHandler(buffer, requested); });
	retro_set_game_seek([](unsigned long requested) { coreInstance->SeekGameFileHandler(requested); });

	retro_init();
}

Snes9XCoreInternal::~Snes9XCoreInternal()
{
	retro_deinit();
	coreInstance = nullptr;
}

bool Snes9XCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
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

bool Snes9XCoreInternal::LoadGame(IStorageFile^ gameFile)
{
	static auto gamePathStr = Converter::PlatformToCPPString(gameFile->Path);
	gameStream = concurrency::create_task(gameFile->OpenAsync(FileAccessMode::Read)).get();

	std::vector<unsigned char> gameData;
	gameData.resize(gameStream->Size);
	auto dataArray = Platform::ArrayReference<unsigned char>(gameData.data(), gameStream->Size);

	auto reader = ref new Windows::Storage::Streams::DataReader(gameStream);
	concurrency::create_task(reader->LoadAsync(gameStream->Size)).get();
	reader->ReadBytes(dataArray);

	auto gameInfo = GenerateGameInfo(dataArray);
	if (!retro_load_game(&gameInfo))
	{
		return false;
	}

	retro_system_av_info info;
	retro_get_system_av_info(&info);
	SetAVInfo(info);

	return true;
}

void Snes9XCoreInternal::UnloadGame()
{
	retro_unload_game();
	gameStream = nullptr;
}

void Snes9XCoreInternal::RunFrame()
{
	if (gameStream == nullptr)
		return;

	retro_run();
}

void Snes9XCoreInternal::Reset()
{
	retro_reset();
}

bool Snes9XCoreInternal::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool Snes9XCoreInternal::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}