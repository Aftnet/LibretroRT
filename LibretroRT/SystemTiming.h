#pragma once

#include "libretro.h"

namespace LibretroRT
{
	public ref struct SystemTiming sealed
	{
	private:
		retro_system_timing nativeTiming;

	internal:
		SystemTiming(const retro_system_timing& timing) : nativeTiming(timing) { }

		property retro_system_timing& NativeTiming
		{
			retro_system_timing& get() { return nativeTiming; }
		}

	public:
		SystemTiming() { }

		property double FPS
		{
			double get() { return nativeTiming.fps; }
			void set(double value) { nativeTiming.fps = value; }
		}

		property double SampleRate
		{
			double get() { return nativeTiming.sample_rate; }
			void set(double value) { nativeTiming.sample_rate = value; }
		}
	};
}