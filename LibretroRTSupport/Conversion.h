#pragma once

#include "libretro.h"

extern "C" {
	RETRO_API std::wstring CStringToWString(const char* t_str);
}