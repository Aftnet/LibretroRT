#pragma once

#include "Export.h"

struct retro_system_info;
struct retro_system_av_info;

using namespace Platform;
using namespace LibretroRT;

namespace LibretroRTSupport
{
	class SUPPORT_API CoreHelper
	{
	private:
		String^ name;
		String^ version;
		String^ supportedExtensions;

		GameGeometry^ gameGeometry;
		SystemTiming^ systemTiming;

	public:
		CoreHelper(const retro_system_info& info);
		void SetAVInfo(const retro_system_av_info& info);

		String^ GetName() { return name; }
		String^ GetVersion() { return version; }
		String^ GetSupportedExtensions() { return supportedExtensions; }

		GameGeometry^ GetGameGeometry() { return ref new GameGeometry(gameGeometry); }
		SystemTiming^ GetSystemTiming() { return ref new SystemTiming(systemTiming); }

		static bool DefaultEnvironmentHandler(unsigned cmd, void *data);
	};
}