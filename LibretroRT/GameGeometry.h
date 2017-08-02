#pragma once

namespace LibretroRT
{
	public ref struct GameGeometry sealed
	{
	public:
		GameGeometry();
		GameGeometry(GameGeometry^ input);
		GameGeometry(unsigned baseWidth, unsigned baseHeight, unsigned maxWidth, unsigned maxHeight, float aspectRatio);

		property unsigned BaseWidth;
		property unsigned BaseHeight;
		property unsigned MaxWidth;
		property unsigned MaxHeight;
		property float AspectRatio;

	private:
		void Initialize(unsigned baseWidth, unsigned baseHeight, unsigned maxWidth, unsigned maxHeight, float aspectRatio);
	};
}