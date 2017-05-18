#include "pch.h"
#include "GPGXCore.h"

#include "../LibretroRT/libretro.h"
#include "../LibretroRTSupport/Converter.h"

using namespace GPGXRT;
using namespace LibretroRTSupport;

GPGXCore::GPGXCore()
{
	retro_system_info info;
	retro_get_system_info(&info);
	Name = Converter::CToPlatformString(info.library_name);
}


GPGXCore::~GPGXCore()
{
}

void GPGXRT::GPGXCore::LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream)
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::GPGXCore::UnloadGame()
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::GPGXCore::RunFrame()
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::GPGXCore::Reset()
{
	throw ref new Platform::NotImplementedException();
}
