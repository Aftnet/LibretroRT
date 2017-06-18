#include "pch.h"
#include "file_stream.h"

using namespace std;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

struct RFILE
{
	IStorageFile^ File;
	IRandomAccessStream^ Stream;

	RFILE()
	{
		File = nullptr;
		Stream = nullptr;
	}
};

namespace FileStreamTools
{
	typedef codecvt_byname<wchar_t, char, mbstate_t> localCodecvt;
	wstring_convert<localCodecvt> StringConverter(new localCodecvt("en_US"));

	unordered_map<string, RFILE> CoreFileMapping;
}

long long int filestream_get_size(RFILE *stream)
{
	return stream->Stream->Size;
}

const char *filestream_get_ext(RFILE *stream)
{
	auto ext = FileStreamTools::StringConverter.to_bytes(stream->File->FileType->Data());
	return ext.data();
}

RFILE *filestream_open(const char *path, unsigned mode, ssize_t len)
{
	string pathStr(path);
	auto output = FileStreamTools::CoreFileMapping[path];
	if (output.File == nullptr)
	{
		//This is where the cllbacks to the front end to get an IStorageFile start
		IStorageFile^ file = nullptr;
		//User did not open file, return null
		if (file == nullptr)
		{
			return nullptr;
		}

		output.File = file;
	}
	
	mode = mode & 0x0f;
	auto accessMode = (mode == RFILE_MODE_READ || mode == RFILE_MODE_READ_TEXT) ? FileAccessMode::Read : FileAccessMode::ReadWrite;
	output.Stream = concurrency::create_task(output.File->OpenAsync(accessMode)).get();
	return &output;
}

ssize_t filestream_seek(RFILE *stream, ssize_t offset, int whence)
{
	auto winStream = stream->Stream;
	switch (whence)
	{
	case SEEK_SET:
		winStream->Seek(offset);
	case SEEK_CUR:
		winStream->Seek(winStream->Position + offset);
	case SEEK_END:
		winStream->Seek(winStream->Size - offset);
	}

	return 0;
}

ssize_t filestream_read(RFILE *stream, void *data, size_t len)
{
	auto dataArray = Platform::ArrayReference<unsigned char>((unsigned char*)data, len);
	auto reader = ref new DataReader(stream->Stream);
	auto output = concurrency::create_task(reader->LoadAsync(len)).get();
	reader->ReadBytes(dataArray);
	return output;
}

ssize_t filestream_write(RFILE *stream, const void *data, size_t len)
{
	auto dataArray = Platform::ArrayReference<unsigned char>((unsigned char*)data, len);
	auto writer = ref new DataWriter(stream->Stream);
	writer->WriteBytes(dataArray);
	concurrency::create_task(stream->Stream->FlushAsync()).wait();
	return len;
}

ssize_t filestream_tell(RFILE *stream)
{
	return stream->Stream->Position;
}

void filestream_rewind(RFILE *stream)
{
	stream->Stream->Seek(0);
}

int filestream_close(RFILE *stream)
{
	stream->Stream = nullptr;
	return 0;
}

int filestream_read_file(const char *path, void **buf, ssize_t *len)
{
	ssize_t ret = 0;
	ssize_t content_buf_size = 0;
	void *content_buf = NULL;
	RFILE *file = filestream_open(path, RFILE_MODE_READ, -1);

	if (!file)
	{
		goto error;
	}

	if (filestream_seek(file, 0, SEEK_END) != 0)
		goto error;

	content_buf_size = filestream_tell(file);
	if (content_buf_size < 0)
		goto error;

	filestream_rewind(file);

	content_buf = malloc(content_buf_size + 1);

	if (!content_buf)
		goto error;

	ret = filestream_read(file, content_buf, content_buf_size);
	if (ret < 0)
	{
		goto error;
	}

	filestream_close(file);

	*buf = content_buf;

	/* Allow for easy reading of strings to be safe.
	* Will only work with sane character formatting (Unix). */
	((char*)content_buf)[ret] = '\0';

	if (len)
		*len = ret;

	return 1;

error:
	if (file)
		filestream_close(file);
	if (content_buf)
		free(content_buf);
	if (len)
		*len = -1;
	*buf = NULL;
	return 0;
}

char *filestream_gets(RFILE *stream, char *s, size_t len)
{
	auto reader = ref new DataReader(stream->Stream);
	concurrency::create_task(reader->LoadAsync(len)).wait();
	auto string = reader->ReadString(len);
	auto converted = FileStreamTools::StringConverter.to_bytes(string->Data());
	strcpy_s(s, len, converted.c_str());
	return s;
}

//char *filestream_getline(RFILE *stream);

int filestream_getc(RFILE *stream)
{
	auto reader = ref new DataReader(stream->Stream);
	concurrency::create_task(reader->LoadAsync(1)).wait();
	return reader->ReadByte();
}

int filestream_eof(RFILE *stream)
{
	size_t current_position = filestream_tell(stream);
	size_t end_position = filestream_seek(stream, 0, SEEK_END);

	filestream_seek(stream, current_position, SEEK_SET);

	if (current_position >= end_position)
		return 1;
	return 0;
}

bool filestream_write_file(const char *path, const void *data, ssize_t size)
{
	ssize_t ret = 0;
	RFILE *file = filestream_open(path, RFILE_MODE_WRITE, -1);
	if (!file)
		return false;

	ret = filestream_write(file, data, size);
	filestream_close(file);

	if (ret != size)
		return false;

	return true;
}

int filestream_putc(RFILE *stream, int c)
{
	auto writer = ref new DataWriter(stream->Stream);
	writer->WriteByte(c);
	concurrency::create_task(stream->Stream->FlushAsync()).wait();
	return c;
}

int filestream_flush(RFILE *stream)
{
	concurrency::create_task(stream->Stream->FlushAsync()).wait();
	return 0;
}