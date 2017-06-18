#pragma once

#include "libretro.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

typedef IRandomAccessStream^ (RETRO_CALLCONV *retro_extra_get_file_t)(String^ filePath, FileAccessMode accessMode);

RETRO_API void retro_extra_set_get_file(retro_extra_get_file_t);