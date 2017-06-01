#include "pch.h"
#include "GambatteCore.h"
#include "GambatteCoreInternal.h"

using namespace GambatteRT;
using namespace LibretroRT;

ICore^ GambatteCore::Instance::get()
{
	return GambatteCoreInternal::Instance;
}