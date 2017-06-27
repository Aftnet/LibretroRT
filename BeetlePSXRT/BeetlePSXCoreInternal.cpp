#include "pch.h"
#include "BeetlePSXCoreInternal.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRT/libretro_extra.h"
#include "../LibretroRT_Tools/Converter.h"

using namespace BeetlePSXRT;
using namespace LibretroRT_Tools;

BeetlePSXCoreInternal^ coreInstance = nullptr;

BeetlePSXCoreInternal^ BeetlePSXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetlePSXCoreInternal();

		retro_set_environment([](unsigned cmd, void* data) { return coreInstance->EnvironmentHandler(cmd, data); });
		retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
		retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return coreInstance->RaiseGetInputState(port, device, index, keyId); });
		retro_set_audio_sample([](int16_t left, int16_t right) { coreInstance->SingleAudioFrameHandler(left, right); });
		retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return coreInstance->RaiseRenderAudioFrames(data, numFrames); });
		retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { coreInstance->RaiseRenderVideoFrame(data, width, height, pitch); });
		retro_extra_set_open_file([](String^ filePath, FileAccessMode accessMode) { return coreInstance->OpenFileStream(filePath, accessMode); });
		retro_extra_set_close_file([](IRandomAccessStream^ stream) { coreInstance->CloseFileStream(stream); });
		retro_init();
	}

	return coreInstance;
}

BeetlePSXCoreInternal::BeetlePSXCoreInternal() : LibretroRT_Tools::CoreBase(retro_get_system_info, retro_get_system_av_info,
	retro_load_game, retro_unload_game, retro_run, retro_reset, retro_serialize_size, retro_serialize, retro_unserialize, retro_deinit,
	true, true)
{
	fileDependencies->Append(ref new FileDependency(L"scph5500.bin", L"PlayStation (v3.0 09/09/96 J) BIOS", L"8dd7d5296a650fac7319bce665a6a53c"));
	fileDependencies->Append(ref new FileDependency(L"scph5501.bin", L"PlayStation (v3.0 11/18/96 A) BIOS", L"490f666e1afb15b7362b406ed1cea246"));
	fileDependencies->Append(ref new FileDependency(L"scph5502.bin", L"PlayStation (v3.0 01/06/97 E) BIOS", L"32736f17079d0b2b7024407c39bd3050"));
}

BeetlePSXCoreInternal::~BeetlePSXCoreInternal()
{
	coreInstance = nullptr;
}

bool BeetlePSXCoreInternal::EnvironmentHandler(unsigned cmd, void *data)
{
	if (CoreBase::EnvironmentHandler(cmd, data))
		return true;

	static std::string EnabledValue("enabled");
	static std::string DisabledValue("disabled");

	switch (cmd)
	{
	case RETRO_ENVIRONMENT_GET_VARIABLE:
		auto varptr = (retro_variable*)data;
		if (!strcmp(varptr->key, "beetle_psx_renderer"))
		{
			varptr->value = "software";
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_internal_resolution"))
		{
			varptr->value = "2x";
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_frame_duping_enable"))
		{
			varptr->value = EnabledValue.c_str();
			return true;
		}
		else if(!strcmp(varptr->key, "beetle_psx_cdimagecache"))
		{
			varptr->value = EnabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_cpu_overclock"))
		{
			varptr->value = DisabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_skipbios"))
		{
			varptr->value = DisabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_analog_toggle"))
		{
			varptr->value = DisabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_analog_calibration"))
		{
			varptr->value = EnabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_crop_overscan"))
		{
			varptr->value = EnabledValue.c_str();
			return true;
		}
		else if (!strcmp(varptr->key, "beetle_psx_enable_memcard1"))
		{
			varptr->value = EnabledValue.c_str();
			return true;
		}
	}

	return false;
}