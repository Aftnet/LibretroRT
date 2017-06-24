using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibretroRT.Test
{
    public class StreamProvider : IDisposable
    {
        public const string Scheme = "ROM\\";

        private readonly IStorageFolder RootFolder;
        private readonly List<IRandomAccessStream> OpenStreams = new List<IRandomAccessStream>();

        public StreamProvider(IStorageFolder rootFolder)
        {
            RootFolder = rootFolder;
        }

        public void Dispose()
        {
            foreach(var i in OpenStreams)
            {
                i.Dispose();
            }
        }

        public IRandomAccessStream OpenFileStream(string path, FileAccessMode fileAccess)
        {
            path = path.Substring(Scheme.Length);
            IStorageFile file;
            try
            {
                file = RootFolder.GetFileAsync(path).AsTask().Result;
            }
            catch
            {
                return null;
            }

            var output = file.OpenAsync(fileAccess).AsTask().Result;
            OpenStreams.Add(output);
            return output;
        }

        public void CloseFileStream(IRandomAccessStream stream)
        {
            if (OpenStreams.Contains(stream))
            {
                stream.Dispose();
                OpenStreams.Remove(stream);
            }
        }
    }
}
