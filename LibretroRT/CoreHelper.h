#pragma once

struct retro_system_info;
struct retro_system_av_info;

using namespace Platform;

namespace LibretroRT
{
	ref struct GameGeometry;
	ref struct SystemTiming;

	private ref class CoreHelper sealed
	{
	internal:
		CoreHelper(retro_system_info& info);
		void SetAVInfo(retro_system_av_info& avInfo);

		property String^ Name { String^ get() { return name; } }
		property String^ Version { String^ get() { return version; } }
		property String^ SupportedExtensions { String^ get() { return supportedExtensions; } }
		property LibretroRT::GameGeometry^ GameGeometry { LibretroRT::GameGeometry^ get() { return gameGeometry; } }
		property LibretroRT::SystemTiming^ SystemTiming { LibretroRT::SystemTiming^ get() { return systemTiming; } }

	private:
		String^ name;
		String^ version;
		String^ supportedExtensions;
		LibretroRT::GameGeometry^ gameGeometry;
		LibretroRT::SystemTiming^ systemTiming;
	};
}