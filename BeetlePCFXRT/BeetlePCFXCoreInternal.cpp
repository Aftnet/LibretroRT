#include "pch.h"
#include "BeetlePCFXCoreInternal.h"

#include "libretro.h"

using namespace BeetlePCFXRT;
using namespace LibretroRT_Shared;

BeetlePCFXCoreInternal^ coreInstance = nullptr;

BeetlePCFXCoreInternal^ BeetlePCFXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetlePCFXCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

BeetlePCFXCoreInternal::BeetlePCFXCoreInternal() : CoreBase(true, true, false)
{
	fileDependencies->Append(ref new FileDependency(L"pcfx.rom", L"PC-FX BIOS", L"08e36edbea28a017f79f8d4f7ff9b6d7"));
}

BeetlePCFXCoreInternal::~BeetlePCFXCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}