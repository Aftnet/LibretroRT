#include "pch.h"
#include "GPGXRT.h"

#include "../LibretroRTSupport/Conversion.h"
#include "../LibretroRTSupport/DefaultHandlers.h"

GPGXRT::GPGXRT()
{
}

void GPGXRT::Init()
{
	retro_set_environment(LibretroDefaultEnvironmentHandler);
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::Deinit()
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::Draw()
{
	throw ref new Platform::NotImplementedException();
}

void GPGXRT::UpdateWindowSize(int width, int height)
{
	throw ref new Platform::NotImplementedException();
}
