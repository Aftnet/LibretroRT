#pragma once

using namespace LibretroRT;

namespace MelonDSRT
{
	public ref class MelonDSCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		MelonDSCore() { }
	};
}