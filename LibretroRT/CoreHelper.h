#pragma once

#include "GameGeometry.h"
#include "SystemTiming.h"

#include "libretro.h"

namespace LibretroRT
{
	class CoreHelper
	{
	private:
		Platform::String^ name;
		Platform::String^ version;
		Platform::String^ supportedExtensions;

		GameGeometry^ gameGeometry;
		SystemTiming^ systemTiming;

	public:
		CoreHelper(retro_system_info& info);
		void SetAVInfo(retro_system_av_info& avInfo);

		Platform::String^ GetName();
		Platform::String^ GetVersion();
		Platform::String^ GetSupportedExtensions();
		GameGeometry^ GetGameGeometry();
		SystemTiming^ GetSystemTiming();
	};
}