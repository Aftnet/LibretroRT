#include "pch.h"
#include "FCEUMMCore.h"
#include "FCEUMMCoreInternal.h"

using namespace FCEUMMRT;
using namespace LibretroRT;

ICore^ FCEUMMCore::Instance::get()
{
	return FCEUMMCoreInternal::Instance;
}