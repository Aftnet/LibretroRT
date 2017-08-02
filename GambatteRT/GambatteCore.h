#pragma once

using namespace LibretroRT;

namespace GambatteRT
{
	public ref class GambatteCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		GambatteCore() { }
	};
}