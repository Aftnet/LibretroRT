#include "pch.h"
#include "VBAMCore.h"
#include "VBAMCoreInternal.h"

using namespace VBAMRT;
using namespace LibretroRT;

ICore^ VBAMCore::Instance::get()
{
	return VBAMCoreInternal::Instance;
}