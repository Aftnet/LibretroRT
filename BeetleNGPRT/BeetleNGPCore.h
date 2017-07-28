#pragma once

using namespace LibretroRT;

namespace BeetleNGPRT
{
	public ref class BeetleNGPCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		BeetleNGPCore() { }
	};
}