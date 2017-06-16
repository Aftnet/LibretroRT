#include "pch.h"
#include "BeetlePSXCore.h"
#include "BeetlePSXCoreInternal.h"

using namespace BeetlePSXRT;
using namespace LibretroRT;

ICore^ BeetlePSXCore::Instance::get()
{
	return BeetlePSXCoreInternal::Instance;
}