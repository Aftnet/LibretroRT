#include "pch.h"
#include "NestopiaCore.h"
#include "NestopiaCoreInternal.h"

using namespace NestopiaRT;
using namespace LibretroRT;

ICore^ NestopiaCore::Instance::get()
{
	return NestopiaCoreInternal::Instance;
}