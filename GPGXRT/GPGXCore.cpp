#include "pch.h"
#include "GPGXCore.h"

#include "../LibretroRT/libretro.h"

using namespace GPGXRT;
using namespace LibretroRTSupport;

GPGXCore^ coreInstance = nullptr;

GPGXCore^ GPGXCore::GetInstance()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GPGXCore();
	}
	return coreInstance;
}

GPGXCore::GPGXCore()
{
	retro_system_info info;
	retro_get_system_info(&info);
	SetSystemInfo(info);

	retro_set_input_poll([]() { coreInstance->RaisePollInput(); });
}

GPGXCore::~GPGXCore()
{
	retro_deinit();
	coreInstance = nullptr;
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
