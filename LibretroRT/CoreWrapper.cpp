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
}

void CoreWrapper::LoadGame(Windows::Storage::Streams::IRandomAccessStream ^ gameStream)
{
	core->LoadGame(gameStream);
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


void CoreWrapper::OnRenderAudioFrames(const Platform::Array<short, 1U> ^data, unsigned int numFrames)
{
	RenderAudioFrames(data, numFrames);
}


void CoreWrapper::OnPollInput()
{
	PollInput();
}


short CoreWrapper::OnGetInputState(unsigned int port, InputTypes inputType)
{
	return GetInputState(port, inputType);
}
