#include "pch.h"
#include "YabauseCore.h"
#include "YabauseCoreInternal.h"

using namespace YabauseRT;
using namespace LibretroRT;

ICore^ YabauseCore::Instance::get()
{
	return YabauseCoreInternal::Instance;
}