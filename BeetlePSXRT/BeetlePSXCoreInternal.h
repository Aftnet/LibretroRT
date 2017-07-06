#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace BeetlePSXRT
{
	private ref class BeetlePSXCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetlePSXCoreInternal();

	public:
		static property BeetlePSXCoreInternal^ Instance { BeetlePSXCoreInternal^ get(); }
		virtual ~BeetlePSXCoreInternal();
	};
}