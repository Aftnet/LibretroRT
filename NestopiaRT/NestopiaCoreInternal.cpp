#include "pch.h"
#include "NestopiaCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace NestopiaRT;
using namespace LibretroRT_Tools;

NestopiaCoreInternal^ coreInstance = nullptr;

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

NestopiaCoreInternal::NestopiaCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit)
{
}

NestopiaCoreInternal::~NestopiaCoreInternal()
{
	coreInstance = nullptr;
}

IVectorView<FileDependency^>^ NestopiaCoreInternal::GenerateFileDependencies()
{
	auto output = ref new Vector<FileDependency^>();
	output->Append(ref new FileDependency(L"disksys.rom", L"Famicom Disk System BIOS", L"ca30b50f880eb660a320674ed365ef7a"));
	return output->GetView();
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