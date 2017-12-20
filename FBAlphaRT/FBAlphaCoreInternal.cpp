#include "pch.h"
#include "FBAlphaCoreInternal.h"

#include "libretro.h"

using namespace FBAlphaRT;
using namespace LibretroRT_Shared;

FBAlphaCoreInternal^ coreInstance = nullptr;

FBAlphaCoreInternal^ FBAlphaCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new FBAlphaCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

FBAlphaCoreInternal::FBAlphaCoreInternal() : CoreBase(true, true, true)
{
	fileDependencies->Append(ref new FileDependency(L"neogeo.zip", L"NeoGeo BIOS collection", L"93adcaa22d652417cbc3927d46b11806"));
}

FBAlphaCoreInternal::~FBAlphaCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}