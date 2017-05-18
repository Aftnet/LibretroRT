#pragma once

namespace LibretroRT
{
	public interface class IRenderer
	{
		void Init();
		void Deinit();

		void Draw();
		void UpdateWindowSize(GLsizei width, GLsizei height);
	};
}

