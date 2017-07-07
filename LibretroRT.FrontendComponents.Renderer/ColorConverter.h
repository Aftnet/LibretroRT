#pragma once

using namespace std;

namespace LibretroRT_FrontendComponents_Renderer
{
	class ColorConverter
	{
	public:
		static void InitializeLookupTable();

		inline static void ConvertRGB565ToXRGB8888(byte* output, byte* input, size_t numPixels)
		{
			auto castInput = (uint16*)input;
			auto castOutput = (uint32*)output;
			for (auto i = 0; i < numPixels; i++)
			{
				*castOutput = RGB565LookupTable[*castInput];
				castInput++;
				castOutput++;
			}
		}

	private:
		static const uint32 LookupTableSize = 65536;
		static bool Initialized;
		static uint32 RGB565LookupTable[LookupTableSize];

		ColorConverter();
	};
}
