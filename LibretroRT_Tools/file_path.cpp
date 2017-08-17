#include <stdint.h>
#include "../LibretroRT/libretro_extra.h"
#include "../LibretroRT_Tools/StringConverter.h"

using namespace Platform;
using namespace Windows::Storage;

bool path_is_character_special_internal(const char *path)
{
	return false;
}

bool path_is_valid_internal(const char *path)
{
	auto winPath = LibretroRT_Tools::StringConverter::CPPToPlatformString(path);
	auto stream = OpenFileStreamViaFrontend(winPath, FileAccessMode::Read);
	bool fileFound = stream != nullptr;
	if (fileFound)
	{
		CloseFileStreamViaFrontend(stream);

	}

	return fileFound;
}

int32_t path_get_size_internal(const char *path)
{
	auto winPath = LibretroRT_Tools::StringConverter::CPPToPlatformString(path);
	auto stream = OpenFileStreamViaFrontend(winPath, FileAccessMode::Read);
	int32_t output = stream->Size;
	CloseFileStreamViaFrontend(stream);
	return output;
}

extern "C" {

	bool path_is_character_special(const char *path)
	{
		return path_is_character_special_internal(path);
	}

	bool path_is_valid(const char *path)
	{
		return path_is_valid_internal(path);
	}

	int32_t path_get_size(const char *path)
	{
		return path_get_size_internal(path);
	}
}
