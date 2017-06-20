#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace Snes9XRT
{
	private ref class Snes9XCoreInternal sealed : public CoreBase
	{
	protected private:
		Snes9XCoreInternal();

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data) override;

	public:
		static property Snes9XCoreInternal^ Instance { Snes9XCoreInternal^ get(); }
		virtual ~Snes9XCoreInternal();
	};
}