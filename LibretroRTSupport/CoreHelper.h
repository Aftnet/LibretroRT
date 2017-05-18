#pragma once

#include "Export.h"

struct retro_system_info;

using namespace Platform;

namespace LibretroRTSupport
{
	class SUPPORT_API CoreHelper
	{
	private:
		String^ name;
		String^ version;
		String^ supportedExtensions;

	public:
		CoreHelper(const retro_system_info& info);

		String^ GetName() { return name; }
		String^ GetVersion() { return version; }
		String^ GetSupportedExtensions() { return supportedExtensions; }
	};
}