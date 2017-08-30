#include "pch.h"
#include "FBAlphaCore.h"
#include "FBAlphaCoreInternal.h"

using namespace FBAlphaRT;
using namespace LibretroRT;

ICore^ FBAlphaCore::Instance::get()
{
	return FBAlphaCoreInternal::Instance;
}