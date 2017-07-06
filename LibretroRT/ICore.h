#pragma once

#include "Enums.h"
#include "CoreOption.h"
#include "FileDependency.h"
#include "GameGeometry.h"
#include "SystemTiming.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation::Collections;

namespace LibretroRT
{
	public delegate void RenderVideoFrameDelegate(const Array<uint8>^ frameBuffer, uint32 width, uint32 height, uint32 pitch);
	public delegate void RenderAudioFramesDelegate(const Array<int16>^ data);
	
	public delegate void PollInputDelegate();
	public delegate int16 GetInputStateDelegate(unsigned port, InputTypes inputType);

	public delegate void GeometryChangedDelegate(GameGeometry^ geometry);
	public delegate void TimingChangedDelegate(SystemTiming^ timing);
	public delegate void PixelFormatChangedDelegate(PixelFormats format);

	public delegate IRandomAccessStream^ OpenFileStreamDelegate(String^ path, FileAccessMode fileAccess);
	public delegate void CloseFileStreamDelegate(IRandomAccessStream^ stream);

	/// <summary>
	/// Interface for LibretroRT cores
	/// </summary>
	public interface class ICore
	{
	public:
		property String^ Name { String^ get(); };
		property String^ Version { String^ get(); };
		property IStorageFolder^ SystemFolder { IStorageFolder^ get(); };
		property IStorageFolder^ SaveGameFolder { IStorageFolder^ get(); };
		property IVectorView<String^>^ SupportedExtensions { IVectorView<String^>^ get(); };
		property IMapView<String^, CoreOption^>^ Options { IMapView<String^, CoreOption^>^ get(); };
		property IVectorView<FileDependency^>^ FileDependencies { IVectorView<FileDependency^>^ get(); };

		property PixelFormats PixelFormat { PixelFormats get(); }
		property GameGeometry^ Geometry { GameGeometry^ get(); }
		property SystemTiming^ Timing { SystemTiming^ get(); }

		property unsigned int SerializationSize { unsigned int get(); }

		bool LoadGame(String^ mainGameFilePath);
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
		property OpenFileStreamDelegate^ OpenFileStream;
		property CloseFileStreamDelegate^ CloseFileStream;
	};
}


