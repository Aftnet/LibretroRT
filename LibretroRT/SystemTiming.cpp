#include "pch.h"
#include "SystemTiming.h"

LibretroRT::SystemTiming::SystemTiming()
{
}

LibretroRT::SystemTiming::SystemTiming(SystemTiming ^ input)
{
	Init(input->FPS, input->SampleRate);
}

LibretroRT::SystemTiming::SystemTiming(double fps, double sampleRate)
{
	Init(fps, sampleRate);
}

void LibretroRT::SystemTiming::Init(double fps, double sampleRate)
{
	FPS = fps;
	SampleRate = sampleRate;
}
