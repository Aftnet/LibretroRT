#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace FCEUMMRT
{
	private ref class FCEUMMCoreInternal sealed : public CoreBase
	{
	protected private:
		FCEUMMCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		static property FCEUMMCoreInternal^ Instance { FCEUMMCoreInternal^ get(); }
		virtual ~FCEUMMCoreInternal();
	};
}