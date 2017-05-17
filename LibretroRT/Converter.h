#pragma once

#include "pch.h"

using namespace Platform;

namespace LibretroRT
{
	public ref class Converter sealed
	{
	internal:
		static String^ CToPlatformString(const char* input);
	};
}