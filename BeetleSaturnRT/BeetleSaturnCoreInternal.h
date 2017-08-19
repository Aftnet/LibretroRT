#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace BeetleSaturnRT
{
	private ref class BeetleSaturnCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetleSaturnCoreInternal();

	public:
		static property BeetleSaturnCoreInternal^ Instance { BeetleSaturnCoreInternal^ get(); }
		virtual ~BeetleSaturnCoreInternal();
	};
}