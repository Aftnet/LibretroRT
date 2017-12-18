#pragma once

#include "CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace GambatteRT
{
	private ref class GambatteCoreInternal sealed : public CoreBase
	{
	protected private:
		GambatteCoreInternal();

	public:
		static property GambatteCoreInternal^ Instance { GambatteCoreInternal^ get(); }
		virtual ~GambatteCoreInternal();
	};
}