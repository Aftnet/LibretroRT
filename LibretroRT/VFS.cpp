#include "pch.h"
#include "VFS.h"

using namespace LibretroRT;

String^ VFS::romPath = ref new String(L"ROM\\");
String^ VFS::savePath = ref new String(L"SAVE\\");
String^ VFS::systemPath = ref new String(L"SYSTEM\\");

VFS::VFS()
{
}
