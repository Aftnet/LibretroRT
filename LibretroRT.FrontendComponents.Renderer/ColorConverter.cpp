#include "pch.h"
#include "ColorConverter.h"

using namespace LibretroRT_FrontendComponents_Renderer;

bool ColorConverter::Initialized = false;
vector<uint32> ColorConverter::RGB565LookupTable(LookupTableSize);

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

void ColorConverter::ConvertFrameBufferRGB565ToXRGB8888(uint16* input, unsigned int width, unsigned int height, unsigned int pitch, uint32* output)
{
	if (!Initialized)
	{
		InitializeLookupTable();
	}

	auto inLineStart = (unsigned char*)input;

	for (auto i = 0; i < height; i++)
	{
		auto inShortPtr = (uint16*)inLineStart;

		for (auto j = 0; j < width; j++)
		{
			*output = RGB565LookupTable[inShortPtr[j]];
			output++;
		}

		inLineStart += pitch;
	}
}

ColorConverter::ColorConverter()
{
}
