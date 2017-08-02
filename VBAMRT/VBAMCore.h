#pragma once

using namespace LibretroRT;

namespace VBAMRT
{
	public ref class VBAMCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		VBAMCore() { }
	};
}