#include "pch.h"
#include "GPGXCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT/libretro_extra.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace GPGXRT;
using namespace LibretroRT_Tools;

GPGXCoreInternal^ coreInstance = nullptr;

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
		retro_extra_set_get_file([](String^ filePath, FileAccessMode accessMode) { return coreInstance->GetFileStream(filePath, accessMode); });
		retro_init();
	}

	return coreInstance;
}

GPGXCoreInternal::GPGXCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit)
{
}

GPGXCoreInternal::~GPGXCoreInternal()
{
	coreInstance = nullptr;
}

IVectorView<FileDependency^>^ GPGXCoreInternal::GenerateFileDependencies()
{
	auto output = ref new Vector<FileDependency^>();
	output->Append(ref new FileDependency(L"BIOS_CD_E.bin", L"Mega-CD (Model 1 1.00 Europe) BIOS", L"e66fa1dc5820d254611fdcdba0662372"));
	output->Append(ref new FileDependency(L"BIOS_CD_J.bin", L"Mega-CD (Model 1 1.00 Japan) BIOS", L"278a9397d192149e84e820ac621a8edd"));
	output->Append(ref new FileDependency(L"BIOS_CD_U.bin", L"Mega-CD (Model 1 1.00 USA) BIOS", L"2efd74e3232ff260e371b99f84024f7f"));
	return output->GetView();
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