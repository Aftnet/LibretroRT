#include "pch.h"
#include "GPGXCore.h"

using namespace GPGXRT;

GPGXCore::GPGXCore()
{
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
