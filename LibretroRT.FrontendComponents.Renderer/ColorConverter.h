#pragma once

using namespace std;

namespace LibretroRT_FrontendComponents_Renderer
{
	class ColorConverter
	{
	public:
		static void InitializeLookupTable();
		static void ConvertRGB565ToXRGB8888(byte* output, byte* input, size_t numPixels);

	private:
		static const uint32 LookupTableSize = 65536;
		static bool Initialized;
		static vector<uint32> RGB565LookupTable;

		ColorConverter();
	};
}
