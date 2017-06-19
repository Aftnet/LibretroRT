#pragma once

#include "ICore.h"

using namespace Windows::Storage;

namespace LibretroRT
{
	//This class's only purpose is to expose cores as public WinRT components.
	public ref class CoreWrapper sealed : ICore
	{
	private:
		ICore^ core;

	public:
		CoreWrapper(ICore^ wrappedCore);

		virtual property String^ Name { String^ get() { return core->Name; } }
		virtual property String^ Version { String^ get() { return core->Version; } }
		virtual property String^ SupportedExtensions { String^ get() { return core->SupportedExtensions; } }
		virtual property PixelFormats PixelFormat { PixelFormats get() { return core->PixelFormat; } }
		virtual property GameGeometry^ Geometry { GameGeometry^ get() { return core->Geometry; } }
		virtual property SystemTiming^ Timing { SystemTiming^ get() { return core->Timing; } }
		virtual property unsigned int SerializationSize { unsigned int get() { return core->SerializationSize; } }

		virtual bool LoadGame(IStorageFile^ gameFile);
		virtual void UnloadGame();
		virtual void RunFrame();
		virtual void Reset();

		virtual bool Serialize(WriteOnlyArray<uint8>^ stateData);
		virtual bool Unserialize(const Array<uint8>^ stateData);

		virtual property RenderVideoFrameDelegate ^ RenderVideoFrame;
		virtual property RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual property PollInputDelegate ^ PollInput;
		virtual property GetInputStateDelegate ^ GetInputState;
		virtual property GeometryChangedDelegate^ GeometryChanged;
		virtual property TimingChangedDelegate^ TimingChanged;
		virtual property PixelFormatChangedDelegate^ PixelFormatChanged;
		virtual property GetFileStreamDelegate^ GetFileStream;

		void OnRenderVideoFrame(const Platform::Array<unsigned char, 1U> ^frameBuffer, unsigned int width, unsigned int height, unsigned int pitch);
		void OnRenderAudioFrames(const Platform::Array<short, 1U> ^data);
		void OnPollInput();
		short OnGetInputState(unsigned int port, LibretroRT::InputTypes inputType);
		void OnGeometryChanged(GameGeometry^ geometry);
		void OnTimingChanged(SystemTiming^ timing);
		void OnPixelFormatChanged(PixelFormats format);
		IRandomAccessStream^ OnGetFileStream(String^ path);
	};
}


