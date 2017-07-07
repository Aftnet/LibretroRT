#include "pch.h"
#include "ColorConverter.h"

using namespace LibretroRT_FrontendComponents_Renderer;

bool ColorConverter::Initialized = false;
uint32 ColorConverter::RGB565LookupTable[LookupTableSize];

void ColorConverter::InitializeLookupTable()
{
	if (Initialized)
	{
		return;
	}

	unsigned int r, g, b;
	for (unsigned int i = 0; i < LookupTableSize; i++)
	{
		r = (i >> 11) & 0x1F;
		g = (i >> 5) & 0x3F;
		b = (i & 0x1F);
		
		r = (unsigned int)std::round(r * 255.0 / 31.0);
		g = (unsigned int)std::round(g * 255.0 / 63.0);
		b = (unsigned int)std::round(b * 255.0 / 31.0);

		RGB565LookupTable[i] = 0xFF000000 | r << 16 | g << 8 | b;
	}

	Initialized = true;
}

ColorConverter::ColorConverter()
{
}
