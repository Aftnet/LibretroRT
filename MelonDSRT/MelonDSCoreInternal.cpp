#include "pch.h"
#include "MelonDSCoreInternal.h"

#include "libretro.h"

using namespace MelonDSRT;
using namespace LibretroRT_Shared;

MelonDSCoreInternal^ coreInstance = nullptr;

MelonDSCoreInternal^ MelonDSCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new MelonDSCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

MelonDSCoreInternal::MelonDSCoreInternal() : CoreBase(true, true, false)
{
	fileDependencies->Append(ref new FileDependency(L"bios7.bin", L"Nintendo DS ARM7 BIOS", L"df692a80a5b1bc90728bc3dfc76cd948"));
	fileDependencies->Append(ref new FileDependency(L"bios9.bin", L"Nintendo DS ARM9 BIOS", L"a392174eb3e572fed6447e956bde4b25"));
	fileDependencies->Append(ref new FileDependency(L"firmware.bin", L"Nintendo DS Firmware", L"b10f39a8a5a573753406f9da2e7232c8"));
}

MelonDSCoreInternal::~MelonDSCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}