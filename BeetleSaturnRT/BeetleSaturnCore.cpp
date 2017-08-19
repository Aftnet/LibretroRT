#include "pch.h"
#include "BeetleSaturnCore.h"
#include "BeetleSaturnCoreInternal.h"

using namespace BeetleSaturnRT;
using namespace LibretroRT;

ICore^ BeetleSaturnCore::Instance::get()
{
	return BeetleSaturnCoreInternal::Instance;
}