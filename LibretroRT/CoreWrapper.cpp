#include "pch.h"
#include "CoreWrapper.h"

using namespace LibretroRT;

CoreWrapper::CoreWrapper(ICore^ wrappedCore) :
	core(wrappedCore)
{
	core->RenderVideoFrame = ref new RenderVideoFrameDelegate(this, &CoreWrapper::OnRenderVideoFrame);
	core->RenderAudioFrames = ref new RenderAudioFramesDelegate(this, &CoreWrapper::OnRenderAudioFrames);
	core->PollInput = ref new PollInputDelegate(this, &CoreWrapper::OnPollInput);
	core->GetInputState = ref new GetInputStateDelegate(this, &CoreWrapper::OnGetInputState);
	core->GeometryChanged = ref new GeometryChangedDelegate(this, &CoreWrapper::OnGeometryChanged);
	core->TimingChanged = ref new TimingChangedDelegate(this, &CoreWrapper::OnTimingChanged);
	core->PixelFormatChanged = ref new PixelFormatChangedDelegate(this, &CoreWrapper::OnPixelFormatChanged);
	core->GetFileStream = ref new GetFileStreamDelegate(this, &CoreWrapper::OnGetFileStream);
}

bool CoreWrapper::LoadGame(Windows::Storage::IStorageFile^ gameFile)
{
	return core->LoadGame(gameFile);
}

void CoreWrapper::UnloadGame()
{
	core->UnloadGame();
}

void CoreWrapper::RunFrame()
{
	core->RunFrame();
}

void CoreWrapper::Reset()
{
	core->Reset();
}

bool CoreWrapper::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return core->Serialize(stateData);
}

bool CoreWrapper::Unserialize(const Array<uint8>^ stateData)
{
	return core->Unserialize(stateData);
}

void CoreWrapper::OnRenderVideoFrame(const Platform::Array<unsigned char, 1U> ^frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{
	RenderVideoFrame(frameBuffer, width, height, pitch);
}


void CoreWrapper::OnRenderAudioFrames(const Platform::Array<short, 1U> ^data)
{
	RenderAudioFrames(data);
}


void CoreWrapper::OnPollInput()
{
	PollInput();
}


short CoreWrapper::OnGetInputState(unsigned int port, InputTypes inputType)
{
	return GetInputState(port, inputType);
}

void CoreWrapper::OnGeometryChanged(GameGeometry^ geometry)
{
	GeometryChanged(geometry);
}

void CoreWrapper::OnTimingChanged(SystemTiming^ timing)
{
	TimingChanged(timing);
}

void CoreWrapper::OnPixelFormatChanged(PixelFormats format)
{
	PixelFormatChanged(format);
}

IRandomAccessStream^ CoreWrapper::OnGetFileStream(String^ path, FileAccessMode fileAccess)
{
	return GetFileStream(path, fileAccess);
}