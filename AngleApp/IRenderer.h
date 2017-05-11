#pragma once

namespace AngleApp
{
	public interface class IRenderer
	{
		void Draw();
		void UpdateWindowSize(GLsizei width, GLsizei height);
	};
}

