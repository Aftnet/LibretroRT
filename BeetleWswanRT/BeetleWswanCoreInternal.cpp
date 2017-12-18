#include "pch.h"
#include "BeetleWswanCoreInternal.h"

#include "libretro.h"

using namespace BeetleWswanRT;
using namespace LibretroRT_Shared;

BeetleWswanCoreInternal^ coreInstance = nullptr;

BeetleWswanCoreInternal^ BeetleWswanCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetleWswanCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

BeetleWswanCoreInternal::BeetleWswanCoreInternal() : CoreBase(true, true, false)
{
}

BeetleWswanCoreInternal::~BeetleWswanCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}