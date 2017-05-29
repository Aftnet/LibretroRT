#include "pch.h"
#include "GameGeometry.h"

LibretroRT::GameGeometry::GameGeometry()
{

}

LibretroRT::GameGeometry::GameGeometry(GameGeometry ^ input)
{
	Initialize(input->BaseWidth, input->BaseHeight, input->MaxWidth, input->MaxHeight, input->AspectRatio);
}

LibretroRT::GameGeometry::GameGeometry(unsigned baseWidth, unsigned baseHeight, unsigned maxWidth, unsigned maxHeight, float aspectRatio)
{
	Initialize(baseWidth, baseHeight, maxWidth, maxHeight, aspectRatio);
}

void LibretroRT::GameGeometry::Initialize(unsigned baseWidth, unsigned baseHeight, unsigned maxWidth, unsigned maxHeight, float aspectRatio)
{
	BaseWidth = baseWidth;
	BaseHeight = baseHeight;
	MaxWidth = maxWidth;
	MaxHeight = maxHeight;
	AspectRatio = aspectRatio;
}
