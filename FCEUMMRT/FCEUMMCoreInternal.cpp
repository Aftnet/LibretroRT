#include "pch.h"
#include "FCEUMMCoreInternal.h"

#include "libretro.h"

using namespace FCEUMMRT;
using namespace LibretroRT_Shared;

FCEUMMCoreInternal^ coreInstance = nullptr;

FCEUMMCoreInternal^ FCEUMMCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new FCEUMMCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

FCEUMMCoreInternal::FCEUMMCoreInternal() : CoreBase(false, false, false)
{
}

FCEUMMCoreInternal::~FCEUMMCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}