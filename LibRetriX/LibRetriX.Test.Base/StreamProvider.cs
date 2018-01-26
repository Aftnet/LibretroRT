using Plugin.FileSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;

namespace LibRetriX.Test.Base
{
    public class StreamProvider : IDisposable
    {
        private readonly string handledScheme;
        public string HandledScheme => handledScheme;

        private readonly IDirectoryInfo RootFolder;
        private readonly List<Stream> OpenStreams = new List<Stream>();

        public StreamProvider(string scheme, IDirectoryInfo rootFolder)
        {
            handledScheme = scheme;
            RootFolder = rootFolder;
        }

        public void Dispose()
        {
            foreach (var i in OpenStreams)
            {
                i.Dispose();
            }
        }

        public Stream OpenFileStream(string path, FileAccess fileAccess)
        {
            if (fileAccess != FileAccess.Read)
            {
                var memoryStream = new MemoryStream();
                OpenStreams.Add(memoryStream);
                return memoryStream;
            }

            path = path.Substring(HandledScheme.Length);
            var output = default(Stream);
            try
            {
                var file = RootFolder.GetFileAsync(path).Result;
                output = file.OpenAsync(fileAccess).Result;
            }
            catch
            {
                return null;
            }

            OpenStreams.Add(output);
            return output;
        }

        public void CloseFileStream(Stream stream)
        {
            if (OpenStreams.Contains(stream))
            {
                stream.Dispose();
                OpenStreams.Remove(stream);
            }
        }
    }
}

