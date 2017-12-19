#pragma once

#include "CoreBase.h"

using namespace LibretroRT_Shared;

namespace MelonDSRT
{
	private ref class MelonDSCoreInternal sealed : public CoreBase
	{
	protected private:
		MelonDSCoreInternal();

	public:
		static property MelonDSCoreInternal^ Instance { MelonDSCoreInternal^ get(); }
		virtual ~MelonDSCoreInternal();
	};
}