#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
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