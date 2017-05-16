#pragma once

#include "libretro.h"

extern "C" {
	RETRO_API bool LibretroDefaultEnvironmentHandler(unsigned cmd, void *data);
	RETRO_API size_t LibretroDefaultAudioFramesRenderer(const int16_t *data, size_t frames);
}