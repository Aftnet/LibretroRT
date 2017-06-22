#include "pch.h"
#include "FCEUMMCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace FCEUMMRT;
using namespace LibretroRT_Tools;

FCEUMMCoreInternal^ coreInstance = nullptr;

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

FCEUMMCoreInternal::FCEUMMCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit,
	false, false)
{
}

FCEUMMCoreInternal::~FCEUMMCoreInternal()
{
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