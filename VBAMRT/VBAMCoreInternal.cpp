#include "pch.h"
#include "VBAMCoreInternal.h"

#include "libretro.h"

using namespace VBAMRT;
using namespace LibretroRT_Shared;

VBAMCoreInternal^ coreInstance = nullptr;

VBAMCoreInternal^ VBAMCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new VBAMCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

VBAMCoreInternal::VBAMCoreInternal() : LibretroRT_Shared::CoreBase(false, false, false)
{
}

VBAMCoreInternal::~VBAMCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}