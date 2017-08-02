#pragma once

using namespace LibretroRT;

namespace BeetleWswanRT
{
	public ref class BeetleWswanCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		BeetleWswanCore() { }
	};
}