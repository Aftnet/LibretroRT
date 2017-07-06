#pragma once

using namespace Platform;
using namespace Windows::Foundation::Collections;

namespace LibretroRT
{
	public ref class CoreOption sealed
	{
	public:
		property String^ Description
		{
			String^ get() { return description; };
		}

		property IVectorView<String^>^ Values
		{
			IVectorView<String^>^ get() { return values; };
		}
		
		property unsigned int SelectedValueIx
		{
			unsigned int get() { return selectedValueIx; };
			void set(unsigned int value)
			{
				if (!(value < Values->Size))
				{
					throw ref new InvalidArgumentException();
				}

				selectedValueIx = value;
			}
		}

		CoreOption(String^ description, IVectorView<String^>^ values, unsigned int selectedValueIx);

	private:
		String^ const description;
		IVectorView<String^>^ const values;
		unsigned int selectedValueIx;
	};
}
