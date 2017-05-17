#include "pch.h"

#include "Converter.h"

using namespace LibretroRT;
using namespace Platform;

String^ Converter::CToPlatformString(const char* input)
{
	//setup converter
	typedef std::codecvt_utf8<wchar_t> convert_type;
	std::wstring_convert<convert_type, wchar_t> converter;

	//use converter (.to_bytes: wstr->str, .from_bytes: str->wstr)
	auto wstring = converter.from_bytes(input);
	return ref new String(wstring.c_str());
}