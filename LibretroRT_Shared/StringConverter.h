#pragma once

#include <string>
#include <vector>

using namespace Platform;

namespace LibretroRT_Tools
{
	class StringConverter
	{
	public:
		static String^ CPPToPlatformString(const std::string string);
		static std::string PlatformToCPPString(Platform::String^ string);
		static std::vector<std::string> SplitString(const std::string input, char delimiter);
	};
}