#pragma once

using namespace Platform;

namespace LibretroRT
{
	public ref class VFS sealed
	{
	public:
		property String^ RomPath { String^ get() { return romPath; } };
		property String^ SavePath { String^ get() { return savePath; } };
		property String^ SystemPath { String^ get() { return systemPath; } };

	private:
		static String^ romPath;
		static String^ savePath;
		static String^ systemPath;

		VFS();
	};
}

