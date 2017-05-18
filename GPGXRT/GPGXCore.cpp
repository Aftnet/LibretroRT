#include "pch.h"
#include "GPGXCore.h"

#include "../LibretroRT/libretro.h"

using namespace GPGXRT;

GPGXCore::GPGXCore()
{
	retro_system_info info;
	auto ptr = Platform::UIntPtr();
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
