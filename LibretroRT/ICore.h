#pragma once

#include "Enums.h"
#include "GameGeometry.h"
#include "SystemTiming.h"

using namespace Platform;
using namespace Windows::Storage;

namespace LibretroRT
{
	public delegate void RenderVideoFrameDelegate(const Array<uint8>^ frameBuffer, uint32 width, uint32 height, uint32 pitch);
	public delegate void RenderAudioFramesDelegate(const Array<int16>^ data, uint32 numFrames);
	
	public delegate void PollInputDelegate();
	public delegate int16 GetInputStateDelegate(unsigned port, InputTypes inputType);

	public interface class ICore
	{
	public:
		property String^ Name { String^ get(); };
		property String^ Version { String^ get(); };
		property String^ SupportedExtensions { String^ get(); };

		property PixelFormats PixelFormat { PixelFormats get(); }
		property GameGeometry^ Geometry { GameGeometry^ get(); }
		property SystemTiming^ Timing { SystemTiming^ get(); }

		bool LoadGame(IStorageFile^ gameFile);
		void UnloadGame();

		void RunFrame();
		void Reset();

		event RenderVideoFrameDelegate^ RenderVideoFrame;
		event RenderAudioFramesDelegate^ RenderAudioFrames;

		event PollInputDelegate^ PollInput;
		event GetInputStateDelegate^ GetInputState;
	};
}


