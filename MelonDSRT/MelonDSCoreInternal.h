#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

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