#pragma once

using namespace LibretroRT;

namespace Snes9XRT
{
	public ref class Snes9XCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	};
}