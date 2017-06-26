#include "pch.h"
#include "StringConverter.h"

using namespace Platform;

using namespace LibretroRT_Tools;

String^ StringConverter::CPPToPlatformString(const std::string string)
{
	std::vector<wchar_t> buffer(string.length() + 1);
	size_t numConverted = 0;
	mbstowcs_s(&numConverted, buffer.data(), buffer.size(), string.c_str(), string.length());
	auto pstring = ref new String(buffer.data());
	return pstring;
}

std::string StringConverter::PlatformToCPPString(String^ string)
{
	std::vector<char> buffer(string->Length() + 1);
	size_t numConverted = 0;
	wcstombs_s(&numConverted, buffer.data(), buffer.size(), string->Data(), string->Length());
	std::string cstring(buffer.data());
	return cstring;
}

std::vector<std::string> StringConverter::SplitString(const std::string input, char delimiter)
{
	std::stringstream iss(input);
	std::string s;
	std::vector<std::string> tokens;
	while (getline(iss, s, delimiter))
	{
		tokens.push_back(s);
	}

	return tokens;
}