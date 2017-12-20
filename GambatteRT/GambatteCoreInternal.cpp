#include "pch.h"
#include "GambatteCoreInternal.h"

#include "libretro.h"

using namespace GambatteRT;
using namespace LibretroRT_Shared;

GambatteCoreInternal^ coreInstance = nullptr;

GambatteCoreInternal^ GambatteCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GambatteCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

GambatteCoreInternal::GambatteCoreInternal() : CoreBase(false, false, false)
{
}

GambatteCoreInternal::~GambatteCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}