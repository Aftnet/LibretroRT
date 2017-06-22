using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public interface IFileProvider : IDisposable
    {
        Task<IEnumerable<string>> ListEntriesAsync();
        Task<Stream> GetFileStreamAsync(string path, FileAccess accessType);
    }
}