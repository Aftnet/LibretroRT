#pragma once

using namespace LibretroRT;

namespace BeetlePSXRT
{
	public ref class BeetlePSXCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	};
}