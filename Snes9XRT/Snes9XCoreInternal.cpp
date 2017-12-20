#include "pch.h"
#include "Snes9XCoreInternal.h"

#include "libretro.h"

using namespace Snes9XRT;
using namespace LibretroRT_Shared;

Snes9XCoreInternal^ coreInstance = nullptr;

Snes9XCoreInternal^ Snes9XCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new Snes9XCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

Snes9XCoreInternal::Snes9XCoreInternal() : CoreBase(false, false, false)
{
}

Snes9XCoreInternal::~Snes9XCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}
