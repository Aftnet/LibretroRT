#include "pch.h"
#include "BeetleNGPCoreInternal.h"

#include "libretro.h"

using namespace BeetleNGPRT;
using namespace LibretroRT_Shared;

BeetleNGPCoreInternal^ coreInstance = nullptr;

BeetleNGPCoreInternal^ BeetleNGPCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetleNGPCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

BeetleNGPCoreInternal::BeetleNGPCoreInternal() : CoreBase(true, true, false)
{
}

BeetleNGPCoreInternal::~BeetleNGPCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}