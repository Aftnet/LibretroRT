#pragma once

using namespace LibretroRT;

namespace BeetleSaturnRT
{
	public ref class BeetleSaturnCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		BeetleSaturnCore() { }
	};
}