#pragma once

#include "CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Shared;
using namespace Windows::Storage;

namespace FBAlphaRT
{
	private ref class FBAlphaCoreInternal sealed : public CoreBase
	{
	protected private:
		FBAlphaCoreInternal();

	public:
		static property FBAlphaCoreInternal^ Instance { FBAlphaCoreInternal^ get(); }
		virtual ~FBAlphaCoreInternal();
	};
}