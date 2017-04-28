#pragma once

using namespace LibRetroRT;

namespace GPGX_RT
{
	public ref class GPGXCore sealed : ILibRetroCore
    {
    public:
		static property GPGXCore^ Instance
		{
			GPGXCore^ get()
			{
				if (instance == nullptr)
				{
					instance = ref new GPGXCore();
				}
				return instance;
			}
		}

		virtual property Platform::String ^ Name
		{
			Platform::String^ get() { return name; }
		}

		virtual property Platform::String ^ Version
		{
			Platform::String^ get() { return version; }
		}

		virtual property Platform::String ^ SupportedExtensions
		{
			Platform::String^ get() { return supportedExtensions; }
		}

		virtual property LibRetroRT::AV::SystemAVInfo AVInfo
		{
			LibRetroRT::AV::SystemAVInfo get() { return avInfo; }
		}

		virtual ~GPGXCore();

		virtual event LibRetroRT::ReadDataDelegate ^ ReadData;
		virtual event LibRetroRT::RenderVideoFrameDelegate ^ RenderVideoFrame;
		virtual event LibRetroRT::RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual void StartExecution(unsigned long long dataSize);
		virtual void EndExecution();
		virtual void Reset();

	private:
		static GPGXCore^ instance;

		Platform::String^ name;
		Platform::String^ version;
		Platform::String^ supportedExtensions;
		LibRetroRT::AV::SystemAVInfo avInfo;

		GPGXCore();
	};
}
