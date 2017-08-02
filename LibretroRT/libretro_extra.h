#pragma once

#include "libretro.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

typedef IRandomAccessStream^ (RETRO_CALLCONV *retro_extra_open_file_t)(String^ filePath, FileAccessMode accessMode);
typedef void (RETRO_CALLCONV *retro_extra_close_file_t)(IRandomAccessStream^ stream);

RETRO_API void retro_extra_set_open_file(retro_extra_open_file_t cb);
RETRO_API void retro_extra_set_close_file(retro_extra_close_file_t cb);

extern retro_extra_open_file_t OpenFileStreamViaFrontend;
extern retro_extra_close_file_t CloseFileStreamViaFrontend;
