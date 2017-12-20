#include "pch.h"
#include "NestopiaCoreInternal.h"

#include "libretro.h"

using namespace NestopiaRT;
using namespace LibretroRT_Shared;

NestopiaCoreInternal^ coreInstance = nullptr;

NestopiaCoreInternal^ NestopiaCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new NestopiaCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

NestopiaCoreInternal::NestopiaCoreInternal() : CoreBase(false, false, false)
{
	fileDependencies->Append(ref new FileDependency(L"disksys.rom", L"Famicom Disk System BIOS", L"ca30b50f880eb660a320674ed365ef7a"));
}

NestopiaCoreInternal::~NestopiaCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}