#pragma once

using namespace LibretroRT;

namespace BeetlePCFXRT
{
	public ref class BeetlePCFXCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		BeetlePCFXCore() { }
	};
}