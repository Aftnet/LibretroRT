#include <stdint.h>
#include "../LibretroRT/libretro_extra.h"

retro_extra_get_storage_item_t GetStorageItemViaFrontend;

void retro_extra_set_get_storage_item(retro_extra_get_storage_item_t cb)
{
	GetStorageItemViaFrontend = cb;
}

bool path_is_directory_inner(const char *path)
{
	return false;
}

bool path_is_character_special_inner(const char *path)
{
	throw ref new Platform::NotImplementedException("path_is_character_special not implemented");
	return false;
}

bool path_is_valid_inner(const char *path)
{
	throw ref new Platform::NotImplementedException("path_is_valid not implemented");
	return false;
}

int32_t path_get_size_inner(const char *path)
{
	throw ref new Platform::NotImplementedException("path_get_size not implemented");
	return 0;
}

/**
* path_mkdir_norecurse:
* @dir                : directory
*
* Create directory on filesystem.
*
* Returns: true (1) if directory could be created, otherwise false (0).
**/
bool mkdir_norecurse_inner(const char *dir)
{
	throw ref new Platform::NotImplementedException("mkdir_norecurse not implemented");
}

extern "C"
{
	bool path_is_directory(const char *path)
	{
		return path_is_directory_inner(path);
	}

	bool path_is_character_special(const char *path)
	{
		return path_is_character_special_inner(path);
	}

	bool path_is_valid(const char *path)
	{
		return path_is_valid_inner(path);
	}

	int32_t path_get_size(const char *path)
	{
		return path_get_size_inner(path);
	}

	/**
	* path_mkdir_norecurse:
	* @dir                : directory
	*
	* Create directory on filesystem.
	*
	* Returns: true (1) if directory could be created, otherwise false (0).
	**/
	bool mkdir_norecurse(const char *dir)
	{
		return mkdir_norecurse_inner(dir);
	}
}