#include "pch.h"
#include "BeetlePSXCoreInternal.h"

#include "libretro.h"

using namespace BeetlePSXRT;
using namespace LibretroRT_Shared;

BeetlePSXCoreInternal^ coreInstance = nullptr;

BeetlePSXCoreInternal^ BeetlePSXCoreInternal::Instance::get()
{
	if (coreInstance == nullptr)
	{
		coreInstance = ref new BeetlePSXCoreInternal();
		CoreBase::SingletonInstance = coreInstance;
	}

	return coreInstance;
}

BeetlePSXCoreInternal::BeetlePSXCoreInternal() : LibretroRT_Shared::CoreBase(true, true, false, 1)
{
	fileDependencies->Append(ref new FileDependency(L"scph5500.bin", L"PlayStation (v3.0 09/09/96 J) BIOS", L"8dd7d5296a650fac7319bce665a6a53c"));
	fileDependencies->Append(ref new FileDependency(L"scph5501.bin", L"PlayStation (v3.0 11/18/96 A) BIOS", L"490f666e1afb15b7362b406ed1cea246"));
	fileDependencies->Append(ref new FileDependency(L"scph5502.bin", L"PlayStation (v3.0 01/06/97 E) BIOS", L"32736f17079d0b2b7024407c39bd3050"));
}

BeetlePSXCoreInternal::~BeetlePSXCoreInternal()
{
	coreInstance = nullptr;
	CoreBase::SingletonInstance = nullptr;
}

void BeetlePSXCoreInternal::OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options)
{
	options->Lookup(L"beetle_psx_frame_duping_enable")->SelectedValueIx = 1;
	options->Lookup(L"beetle_psx_analog_calibration")->SelectedValueIx = 1;
	options->Lookup(L"beetle_psx_skipbios")->SelectedValueIx = 1;
}