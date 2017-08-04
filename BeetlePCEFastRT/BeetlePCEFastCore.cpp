#include "pch.h"
#include "BeetlePCEFastCore.h"
#include "BeetlePCEFastCoreInternal.h"

using namespace BeetlePCEFastRT;
using namespace LibretroRT;

ICore^ BeetlePCEFastCore::Instance::get()
{
	return BeetlePCEFastCoreInternal::Instance;
}