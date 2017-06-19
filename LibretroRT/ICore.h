#pragma once

#include "Enums.h"
#include "GameGeometry.h"
#include "SystemTiming.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

namespace LibretroRT
{
	public delegate void RenderVideoFrameDelegate(const Array<uint8>^ frameBuffer, uint32 width, uint32 height, uint32 pitch);
	public delegate void RenderAudioFramesDelegate(const Array<int16>^ data);
	
	public delegate void PollInputDelegate();
	public delegate int16 GetInputStateDelegate(unsigned port, InputTypes inputType);

	public delegate void GeometryChangedDelegate(GameGeometry^ geometry);
	public delegate void TimingChangedDelegate(SystemTiming^ timing);
	public delegate void PixelFormatChangedDelegate(PixelFormats format);

	public delegate IRandomAccessStream^ GetFileStreamDelegate(String^ path);

	/// <summary>
	/// Interface for LibretroRT cores
	/// </summary>
	public interface class ICore
	{
	public:
		property String^ Name { String^ get(); };
		property String^ Version { String^ get(); };
		property String^ SupportedExtensions { String^ get(); };

		property PixelFormats PixelFormat { PixelFormats get(); }
		property GameGeometry^ Geometry { GameGeometry^ get(); }
		property SystemTiming^ Timing { SystemTiming^ get(); }

		property unsigned int SerializationSize { unsigned int get(); }

		bool LoadGame(IStorageFile^ gameFile);
		void UnloadGame();

		void RunFrame();
		void Reset();

		bool Serialize(WriteOnlyArray<uint8>^ stateData);
		bool Unserialize(const Array<uint8>^ stateData);

		property RenderVideoFrameDelegate^ RenderVideoFrame;
		property RenderAudioFramesDelegate^ RenderAudioFrames;
		property PollInputDelegate^ PollInput;
		property GetInputStateDelegate^ GetInputState;
		property GeometryChangedDelegate^ GeometryChanged;
		property TimingChangedDelegate^ TimingChanged;
		property PixelFormatChangedDelegate^ PixelFormatChanged;
		property GetFileStreamDelegate^ GetFileStream;
	};
}


