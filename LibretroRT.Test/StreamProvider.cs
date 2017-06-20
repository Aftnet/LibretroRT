using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibretroRT.Test
{
    public class StreamProvider
    {
        public IDictionary<string, IStorageFile> FileMappings { get; private set; }

        public StreamProvider()
        {
            FileMappings = new Dictionary<string, IStorageFile>();
        }

        public string AddFile(IStorageFile file)
        {
            var key = $"ROM\\{file.Name}";
            FileMappings.Add(key, file);
            return key;
        }

        public IRandomAccessStream GetFileStream(string path, FileAccessMode fileAccess)
        {
            if (!FileMappings.ContainsKey(path))
            {
                return null;
            }

            var file = FileMappings[path];
            var output = file.OpenAsync(fileAccess).AsTask().Result;
            return output;
        }
    }
}
