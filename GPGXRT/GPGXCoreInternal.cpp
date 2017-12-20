#include "pch.h"
#include "GPGXCoreInternal.h"

#include "libretro.h"

using namespace GPGXRT;
using namespace LibretroRT_Shared;

GPGXCoreInternal^ coreInstance = nullptr;

GPGXCoreInternal^ GPGXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new GPGXCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

GPGXCoreInternal::GPGXCoreInternal() : CoreBase(true, true, false)
{
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_E.bin", L"Mega-CD (Model 1 1.00 Europe) BIOS", L"e66fa1dc5820d254611fdcdba0662372"));
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_J.bin", L"Mega-CD (Model 1 1.00 Japan) BIOS", L"278a9397d192149e84e820ac621a8edd"));
	fileDependencies->Append(ref new FileDependency(L"BIOS_CD_U.bin", L"Mega-CD (Model 1 1.00 USA) BIOS", L"2efd74e3232ff260e371b99f84024f7f"));
}

GPGXCoreInternal::~GPGXCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}