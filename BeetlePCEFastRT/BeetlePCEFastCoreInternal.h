#pragma once

#include "CoreBase.h"

using namespace LibretroRT_Shared;

namespace BeetlePCEFastRT
{
	private ref class BeetlePCEFastCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetlePCEFastCoreInternal();

	public:
		static property BeetlePCEFastCoreInternal^ Instance { BeetlePCEFastCoreInternal^ get(); }
		virtual ~BeetlePCEFastCoreInternal();
	};
}