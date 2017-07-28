#include "pch.h"
#include "BeetleWswanCore.h"
#include "BeetleWswanCoreInternal.h"

using namespace BeetleWswanRT;
using namespace LibretroRT;

ICore^ BeetleWswanCore::Instance::get()
{
	return BeetleWswanCoreInternal::Instance;
}