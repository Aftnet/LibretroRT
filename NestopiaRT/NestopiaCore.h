#pragma once

using namespace LibretroRT;

namespace NestopiaRT
{
	public ref class NestopiaCore sealed
	{
	public:
		static property ICore^ Instance { ICore^ get(); }
	};
}