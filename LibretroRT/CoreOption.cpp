#include "pch.h"
#include "CoreOption.h"

using namespace LibretroRT;

CoreOption::CoreOption(String^ description, IVectorView<String^>^ values, unsigned int selectedValueIx):
	description(description),
	values(values)
{
	if (values->Size < 1)
	{
		throw ref new InvalidArgumentException();
	}

	SelectedValueIx = selectedValueIx;
}
