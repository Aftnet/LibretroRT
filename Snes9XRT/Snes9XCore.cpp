#include "pch.h"
#include "Snes9XCore.h"
#include "Snes9XCoreInternal.h"

using namespace Snes9XRT;
using namespace LibretroRT;

ICore^ Snes9XCore::Instance::get()
{
	return Snes9XCoreInternal::Instance;
}