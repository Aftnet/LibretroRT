#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace BeetleWswanRT
{
	private ref class BeetleWswanCoreInternal sealed : public CoreBase
	{
	protected private:
		BeetleWswanCoreInternal();

	public:
		static property BeetleWswanCoreInternal^ Instance { BeetleWswanCoreInternal^ get(); }
		virtual ~BeetleWswanCoreInternal();
	};
}