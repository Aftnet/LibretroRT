#pragma once

#include "CoreBase.h"

using namespace LibretroRT_Shared;

namespace BeetlePCFXRT
{
	private ref class BeetlePCFXCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetlePCFXCoreInternal();

	public:
		static property BeetlePCFXCoreInternal^ Instance { BeetlePCFXCoreInternal^ get(); }
		virtual ~BeetlePCFXCoreInternal();
	};
}