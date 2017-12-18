#pragma once

#include "CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;

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