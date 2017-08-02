#pragma once

using namespace LibretroRT;

namespace FCEUMMRT
{
	public ref class FCEUMMCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		FCEUMMCore() { }
	};
}