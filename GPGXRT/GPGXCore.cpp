#include "pch.h"
#include "GPGXCore.h"
#include "GPGXCoreInternal.h"

using namespace GPGXRT;
using namespace LibretroRT;

ICore^ GPGXCore::Instance::get()
{
	return GPGXCoreInternal::Instance;
}