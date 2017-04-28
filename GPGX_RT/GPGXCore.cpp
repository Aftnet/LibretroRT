#include "pch.h"
#include "GPGXCore.h"

#include "../GPGX/src/libretro/libretro.h"

using namespace Platform;

namespace GPGX_RT
{
	GPGXCore^ GPGXCore::instance = nullptr;

	GPGXCore::GPGXCore()
	{
		retro_init();
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
}
