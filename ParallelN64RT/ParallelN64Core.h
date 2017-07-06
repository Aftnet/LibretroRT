#pragma once

using namespace LibretroRT;

namespace ParallelN64RT
{
	public ref class ParallelN64Core sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	private:
		ParallelN64Core() { }
	};
}