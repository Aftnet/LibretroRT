#include "pch.h"
#include "CoreHelper.h"
#include "Converter.h"

using namespace LibretroRT;

CoreHelper::CoreHelper(retro_system_info& info):
	name(Converter::CToPlatformString(info.library_name)),
	version(Converter::CToPlatformString(info.library_version)),
	supportedExtensions(Converter::CToPlatformString(info.valid_extensions))
{
}

void LibretroRT::CoreHelper::SetAVInfo(retro_system_av_info & avInfo)
{
	gameGeometry = ref new GameGeometry(avInfo.geometry);
	systemTiming = ref new SystemTiming(avInfo.timing);
}

Platform::String ^ LibretroRT::CoreHelper::GetName()
{
	return name;
}

Platform::String ^ LibretroRT::CoreHelper::GetVersion()
{
	return version;
}

Platform::String ^ LibretroRT::CoreHelper::GetSupportedExtensions()
{
	return supportedExtensions;
}

GameGeometry ^ LibretroRT::CoreHelper::GetGameGeometry()
{
	return gameGeometry;
}

SystemTiming ^ LibretroRT::CoreHelper::GetSystemTiming()
{
	return systemTiming;
}
