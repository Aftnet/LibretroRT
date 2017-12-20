#pragma once

#include "../LibretroRT_Shared/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace GPGXRT
{
	private ref class GPGXCoreInternal sealed : public CoreBase
	{
	protected private:
		GPGXCoreInternal();

	public:
		static property GPGXCoreInternal^ Instance { GPGXCoreInternal^ get(); }
		virtual ~GPGXCoreInternal();
	};
}