#pragma once

using namespace Platform;

namespace LibretroRT
{
	public ref class FileDependency sealed
	{
	public:
		property String^ Name
		{
			String^ get() { return name; }
		private:
			void set(String^ value) { name = value; }
		};

		property String^ Description
		{
			String^ get() { return description; }
		private:
			void set(String^ value) { description = value; }
		};

		property String^ MD5
		{
			String^ get() { return md5; }
		private:
			void set(String^ value) { md5 = value; }
		};

		FileDependency(String^ name, String^ description, String^ md5);

	private:
		String^ name;
		String^ description;
		String^ md5;
	};
}


