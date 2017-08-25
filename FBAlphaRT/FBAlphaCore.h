#pragma once

using namespace LibretroRT;

namespace FBAlphaRT
{
	public ref class FBAlphaCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		FBAlphaCore() { }
	};
}