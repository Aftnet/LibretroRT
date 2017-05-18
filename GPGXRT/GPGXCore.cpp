#include "pch.h"
#include "GPGXCore.h"

#include "../LibretroRT/libretro.h"

using namespace GPGXRT;
using namespace LibretroRTSupport;

GPGXCore::GPGXCore()
{
	retro_system_info info;
	retro_get_system_info(&info);
	helper = std::make_shared<CoreHelper>(CoreHelper(info));

	retro_set_environment(CoreHelper::DefaultEnvironmentHandler);
	retro_init();
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
