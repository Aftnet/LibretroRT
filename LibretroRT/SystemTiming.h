#pragma once

#include "libretro.h"

namespace LibretroRT
{
	public ref struct SystemTiming sealed
	{
	public:
		SystemTiming();
		SystemTiming(SystemTiming^ input);
		SystemTiming(double fps, double sampleRate);

		property double FPS;
		property double SampleRate;

	private:
		void Init(double fps, double sampleRate);
	};
}