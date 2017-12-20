#pragma once

#include "CoreBase.h"

using namespace LibretroRT_Shared;

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