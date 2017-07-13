#pragma once

using namespace LibretroRT;

namespace YabauseRT
{
	public ref class YabauseCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		YabauseCore() { }
	};
}