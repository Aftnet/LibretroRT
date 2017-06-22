#include "pch.h"
#include "FileDependency.h"

using namespace LibretroRT;

FileDependency::FileDependency(String^ name, String^ description, String^ md5)
{
	Name = name;
	Description = description;
	MD5 = md5;
}
