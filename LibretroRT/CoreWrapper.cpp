#include "pch.h"
#include "CoreWrapper.h"

using namespace LibretroRT;

CoreWrapper::CoreWrapper(ICore^ wrappedCore) :
	core(wrappedCore)
{
	core->RenderVideoFrame += ref new RenderVideoFrameDelegate(this, &CoreWrapper::OnRenderVideoFrame);
	core->RenderAudioFrames += ref new RenderAudioFramesDelegate(this, &CoreWrapper::OnRenderAudioFrames);
	core->PollInput += ref new PollInputDelegate(this, &CoreWrapper::OnPollInput);
	core->GetInputState += ref new GetInputStateDelegate(this, &CoreWrapper::OnGetInputState);
	core->GameGeometryChanged += ref new GameGeometryChangedDelegate(this, &CoreWrapper::OnGameGeometryChanged);
	core->SystemTimingChanged += ref new SystemTimingChangedDelegate(this, &CoreWrapper::OnSystemTimingChanged);
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

void CoreWrapper::OnGameGeometryChanged(GameGeometry^ geometry)
{
	GameGeometryChanged(geometry);
}

void CoreWrapper::OnSystemTimingChanged(SystemTiming^ timing)
{
	SystemTimingChanged(timing);
}