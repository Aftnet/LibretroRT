#pragma once

using namespace std;

namespace LibretroRT_FrontendComponents_Renderer
{
	class ColorConverter
	{
	public:
		static void InitializeLookupTable();
		static void ConvertFrameBufferRGB565ToXRGB8888(uint16* input, unsigned int width, unsigned int height, unsigned int pitch, uint32* output);

	private:
		static const uint32 LookupTableSize = 65536;
		static bool Initialized;
		static vector<uint32> RGB565LookupTable;

		ColorConverter();
	};
}
