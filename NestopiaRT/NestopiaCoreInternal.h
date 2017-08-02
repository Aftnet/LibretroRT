#pragma once

#include "../LibretroRT_Tools/CoreBase.h"

using namespace Platform;
using namespace LibretroRT_Tools;
using namespace Windows::Storage;

namespace NestopiaRT
{
	private ref class NestopiaCoreInternal sealed : public CoreBase
	{
	protected private:
		NestopiaCoreInternal();

	public:
		static property NestopiaCoreInternal^ Instance { NestopiaCoreInternal^ get(); }
		virtual ~NestopiaCoreInternal();
	};
}