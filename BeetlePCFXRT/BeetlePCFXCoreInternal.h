#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

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