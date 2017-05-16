#pragma once

using namespace LibretroRT;

ref class GPGXRT sealed : IRenderer
{
public:
	GPGXRT();

	// Inherited via IRenderer
	virtual void Init();
	virtual void Deinit();
	virtual void Draw();
	virtual void UpdateWindowSize(int width, int height);
};

