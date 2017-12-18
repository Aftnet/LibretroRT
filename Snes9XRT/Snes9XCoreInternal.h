#pragma once

#include "CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace Snes9XRT
{
	private ref class Snes9XCoreInternal sealed : public CoreBase
	{
	protected private:
		Snes9XCoreInternal();

	public:
		static property Snes9XCoreInternal^ Instance { Snes9XCoreInternal^ get(); }
		virtual ~Snes9XCoreInternal();
	};
}