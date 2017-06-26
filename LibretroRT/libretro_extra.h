#pragma once

#include "libretro.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

typedef IStorageItem^ (RETRO_CALLCONV *retro_extra_get_storage_item_t)(String^ path);
typedef IRandomAccessStream^ (RETRO_CALLCONV *retro_extra_open_file_t)(String^ filePath, FileAccessMode accessMode);
typedef void (RETRO_CALLCONV *retro_extra_close_file_t)(IRandomAccessStream^ stream);

RETRO_API void retro_extra_set_get_storage_item(retro_extra_get_storage_item_t);
RETRO_API void retro_extra_set_open_file(retro_extra_open_file_t);
RETRO_API void retro_extra_set_close_file(retro_extra_close_file_t);