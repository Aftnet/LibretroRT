#include "pch.h"
#include "Class1.h"

#include "../GPGX/src/libretro/libretro.h"

#include <Windows.h>

using namespace GPGX_RT;
using namespace Platform;

Class1::Class1()
{
}

Platform::String^ Class1::GetVersion()
{
	retro_system_info sysInfo;
	retro_get_system_info(&sysInfo);

	std::string versionStr(sysInfo.library_version);
	std::wstring wVersionStr(versionStr.begin(), versionStr.end());
	Platform::String^ output = ref new Platform::String(wVersionStr.c_str());
	return output;
}