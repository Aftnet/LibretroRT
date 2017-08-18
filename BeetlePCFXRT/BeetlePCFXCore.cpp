#include "pch.h"
#include "BeetlePCFXCore.h"
#include "BeetlePCFXCoreInternal.h"

using namespace BeetlePCFXRT;
using namespace LibretroRT;

ICore^ BeetlePCFXCore::Instance::get()
{
	return BeetlePCFXCoreInternal::Instance;
}