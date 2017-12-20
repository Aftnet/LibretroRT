#pragma once

#include "CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace FCEUMMRT
{
	private ref class FCEUMMCoreInternal sealed : public CoreBase
	{
	protected private:
		FCEUMMCoreInternal();

	public:
		static property FCEUMMCoreInternal^ Instance { FCEUMMCoreInternal^ get(); }
		virtual ~FCEUMMCoreInternal();
	};
}