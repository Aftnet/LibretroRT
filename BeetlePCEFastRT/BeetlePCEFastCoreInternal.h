#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

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