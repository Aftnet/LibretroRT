#include "pch.h"
#include "GPGXCore.h"

#include <locale>
#include <codecvt>
#include <string>

#include "../GPGX/src/libretro/libretro.h"

using namespace Platform;

namespace GPGX_RT
{
	GPGXCore^ GPGXCore::instance = nullptr;

	GPGXCore::GPGXCore()
	{
		retro_set_environment(&EnvironmentHandler);
		retro_init();

		retro_system_info info;
		retro_get_system_info(&info);

		name = CStringToPlatformString(info.library_name);
		version = CStringToPlatformString(info.library_version);
		supportedExtensions = CStringToPlatformString(info.valid_extensions);
	}

	GPGXCore::~GPGXCore()
	{
		if (instance != nullptr)
		{
			retro_deinit();
		}
		instance = nullptr;
	}

	void GPGXCore::StartExecution(unsigned long long dataSize)
	{
		throw ref new Platform::NotImplementedException();
	}

	void GPGXCore::EndExecution()
	{
		throw ref new Platform::NotImplementedException();
	}

	void GPGXCore::Reset()
	{
		throw ref new Platform::NotImplementedException();
	}

	bool GPGXCore::EnvironmentHandler(unsigned cmd, void *data)
	{
		return true;
	}

	Platform::String^ GPGXCore::CStringToPlatformString(const char* string)
	{
		std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
		std::wstring wide = converter.from_bytes(string);
		return ref new Platform::String(wide.c_str());
	}
}
