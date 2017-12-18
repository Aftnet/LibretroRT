#include "pch.h"
#include "BeetlePCEFastCoreInternal.h"

#include "libretro.h"

using namespace BeetlePCEFastRT;
using namespace LibretroRT_Shared;

BeetlePCEFastCoreInternal^ coreInstance = nullptr;

BeetlePCEFastCoreInternal^ BeetlePCEFastCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetlePCEFastCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

BeetlePCEFastCoreInternal::BeetlePCEFastCoreInternal() : CoreBase(true, true, false)
{
	fileDependencies->Append(ref new FileDependency(L"syscard3.pce", L"PC Engine CD BIOS", L"ff1a674273fe3540ccef576376407d1d"));
}

BeetlePCEFastCoreInternal::~BeetlePCEFastCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}