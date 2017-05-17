#pragma once

#include "libretro.h"

namespace LibretroRT
{
	public ref struct GameGeometry sealed
	{
	private:
		retro_game_geometry nativeGeometry;

	internal:
		GameGeometry(const retro_game_geometry& geometry) : nativeGeometry(geometry) { }

		property retro_game_geometry& NativeGeometry
		{
			retro_game_geometry& get() { return nativeGeometry; }
		}

	public:
		GameGeometry() { }

		property unsigned BaseWidth
		{
			unsigned get() { return nativeGeometry.base_width; }
			void set(unsigned value) { nativeGeometry.base_width = value; }
		}

		property unsigned BaseHeight
		{
			unsigned get() { return nativeGeometry.base_height; }
			void set(unsigned value) { nativeGeometry.base_height = value; }
		}

		property unsigned MaxWidth
		{
			unsigned get() { return nativeGeometry.max_width; }
			void set(unsigned value) { nativeGeometry.max_width = value; }
		}

		property unsigned MaxHeight
		{
			unsigned get() { return nativeGeometry.max_height; }
			void set(unsigned value) { nativeGeometry.max_height = value; }
		}

		property float AspectRatio
		{
			float get() { return nativeGeometry.aspect_ratio; }
			void set(float value) { nativeGeometry.aspect_ratio = value; }
		}
	};
}