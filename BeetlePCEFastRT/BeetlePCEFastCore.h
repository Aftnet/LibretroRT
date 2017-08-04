#pragma once

using namespace LibretroRT;

namespace BeetlePCEFastRT
{
	public ref class BeetlePCEFastCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		BeetlePCEFastCore() { }
	};
}