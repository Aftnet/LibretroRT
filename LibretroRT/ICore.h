#pragma once

#include "Devices.h"
#include "GameGeometry.h"
#include "SystemTiming.h"

using namespace Platform;

namespace LibretroRT
{
	public delegate void RenderVideoFrameDelegate(const Array<uint8>^ frameBuffer, uint32 width, uint32 height, uint32 pitch);
	public delegate void RenderAudioFramesDelegate(const Array<int16>^ data, uint32 numFrames);
	
	public delegate void PollInputDelegate();
	public delegate int16 GetInputStateDelegate(unsigned port, DeviceType deviceType, unsigned index, unsigned keyId);

	public interface class ICore
	{
	public:
		property String^ Name { String^ get(); };
		property String^ Version { String^ get(); };
		property String^ SupportedExtensions { String^ get(); };

		property GameGeometry^ Geometry { GameGeometry^ get(); }
		property SystemTiming^ Timing { SystemTiming^ get(); }

		void LoadGame(Windows::Storage::Streams::IRandomAccessStream^ gameStream);
		void UnloadGame();

		void RunFrame();
		void Reset();

		event RenderVideoFrameDelegate^ RenderVideoFrame;
		event RenderAudioFramesDelegate^ RenderAudioFrames;

		event PollInputDelegate^ PollInput;
		event GetInputStateDelegate^ GetInputState;
	};
}


