#include "pch.h"
#include "ParallelN64Core.h"
#include "ParallelN64CoreInternal.h"

using namespace ParallelN64RT;
using namespace LibretroRT;

ICore^ ParallelN64Core::Instance::get()
{
	return ParallelN64CoreInternal::Instance;
}