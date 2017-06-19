﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public interface IFileProvider : IDisposable
    {
        Task<Stream> GetFileStreamAsync(string path, FileAccess accessType);
    }
}