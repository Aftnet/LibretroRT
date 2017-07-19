#include "pch.h"
#include "MelonDSCore.h"
#include "MelonDSCoreInternal.h"

using namespace MelonDSRT;
using namespace LibretroRT;

ICore^ MelonDSCore::Instance::get()
{
	return MelonDSCoreInternal::Instance;
}