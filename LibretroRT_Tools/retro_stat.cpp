#include <stdint.h>
#include "../LibretroRT/libretro_extra.h"

using namespace Platform;
using namespace Windows::Storage;

bool path_is_directory_internal(const char *path)
{
	throw ref new Platform::NotImplementedException("path_is_directory_internal");
}

bool path_is_character_special_internal(const char *path)
{
	return false;
}

bool path_is_valid_internal(const char *path)
{
	return true;
}

int32_t path_get_size_internal(const char *path)
{
	//auto winPath = ExtraTools::StringConverter.from_B
	//auto stream = OpenFileStreamViaFrontend(winPath, FileAccessMode::Read);
	throw ref new Platform::NotImplementedException("path_get_size_internal");
}

bool mkdir_norecurse_internal(const char *dir)
{
	throw ref new Platform::NotImplementedException("mkdir_norecurse_internal");
}

extern "C" {

	bool path_is_directory(const char *path)
	{
		return path_is_directory_internal(path);
	}

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

	bool mkdir_norecurse(const char *dir)
	{
		return mkdir_norecurse_internal(dir);
	}
}
