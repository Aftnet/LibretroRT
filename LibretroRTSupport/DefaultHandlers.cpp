#include "pch.h"

#include "DefaultHandlers.h"

using namespace Windows::Media::Audio;

bool LibretroDefaultEnvironmentHandler(unsigned cmd, void *data)
{
	return true;
}

size_t LibretroDefaultAudioFramesRenderer(const int16_t *data, size_t frames)
{
	return 0;
}