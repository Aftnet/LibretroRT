using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibretroRT.Test
{
    public class StreamProvider : IDisposable
    {
        private readonly List<IRandomAccessStream> OpenStreams = new List<IRandomAccessStream>();
        public IDictionary<string, IStorageFile> FileMappings { get; private set; }

        public StreamProvider()
        {
            FileMappings = new Dictionary<string, IStorageFile>();
        }

        public void Dispose()
        {
            foreach(var i in OpenStreams)
            {
                i.Dispose();
            }
        }

        public string AddFile(IStorageFile file)
        {
            var key = $"ROM\\{file.Name}";
            FileMappings.Add(key, file);
            return key;
        }

        public IRandomAccessStream OpenFileStream(string path, FileAccessMode fileAccess)
        {
            if (!FileMappings.ContainsKey(path))
            {
                return null;
            }

            var file = FileMappings[path];
            var output = file.OpenAsync(fileAccess).AsTask().Result;
            OpenStreams.Add(output);
            return output;
        }

        public void CloseFileStream(IRandomAccessStream stream)
        {
            if (OpenStreams.Contains(stream))
            {
                stream.Dispose();
            }

            OpenStreams.Remove(stream);
        }
    }
}
