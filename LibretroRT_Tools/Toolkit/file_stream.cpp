#include "pch.h"
#include "file_stream.h"

using namespace std;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

typedef std::codecvt_byname<wchar_t, char, std::mbstate_t> localCodecvt;
std::wstring_convert<localCodecvt> stringConverter(new localCodecvt("en_US"));

struct RFILE
{
	IStorageFile^ File;
	IRandomAccessStream^ Stream;
};

extern map<string, IStorageFile^> CoreFileMapping;

map<RFILE, IStorageFile^> OpenFilesMapping;

long long int filestream_get_size(RFILE *stream)
{
	return stream->Stream->Size;
}

const char *filestream_get_ext(RFILE *stream)
{
	auto ext = stringConverter.to_bytes(stream->File->FileType->Data());
	return ext.data();
}

RFILE *filestream_open(const char *path, unsigned mode, ssize_t len)
{
	string pathStr(path);
	return nullptr;
}