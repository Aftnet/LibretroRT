#pragma once

#include "libretro.h"

using namespace Platform;
using namespace Windows::Storage;

typedef IStorageFile^ (RETRO_CALLCONV *retro_extra_get_file_t)(Platform::String^ filePath);

RETRO_API void retro_extra_set_get_file(retro_extra_get_file_t);