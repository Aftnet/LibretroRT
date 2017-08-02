#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace BeetleNGPRT
{
	private ref class BeetleNGPCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetleNGPCoreInternal();

	public:
		static property BeetleNGPCoreInternal^ Instance { BeetleNGPCoreInternal^ get(); }
		virtual ~BeetleNGPCoreInternal();
	};
}