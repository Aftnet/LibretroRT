#pragma once

using namespace Platform;
using namespace Windows::Foundation::Collections;

namespace LibretroRT
{
	public ref class CoreOptionDescription sealed
	{
	public:
		property String^ Key
		{
			String^ get() { return key; };
		}

		property String^ Description
		{
			String^ get() { return description; };
		}

		property IVectorView<String^>^ AllowedValues
		{
			IVectorView<String^>^ get() { return allowedValues; };
		}
		
		CoreOptionDescription(String^ key, String^ description, IVectorView<String^>^ allowedValues);

	private:
		String^ const key;
		String^ const description;
		IVectorView<String^>^ const allowedValues;
	};
}
