#include "pch.h"
#include "BeetleNGPCore.h"
#include "BeetleNGPCoreInternal.h"

using namespace BeetleNGPRT;
using namespace LibretroRT;

ICore^ BeetleNGPCore::Instance::get()
{
	return BeetleNGPCoreInternal::Instance;
}