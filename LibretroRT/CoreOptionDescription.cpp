#include "pch.h"
#include "CoreOptionDescription.h"

using namespace LibretroRT;

CoreOptionDescription::CoreOptionDescription(String^ key, String^ description, IVectorView<String^>^ allowedValues):
	key(key),
	description(description),
	allowedValues(allowedValues)
{
}
