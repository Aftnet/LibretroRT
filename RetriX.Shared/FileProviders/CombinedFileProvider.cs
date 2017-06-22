using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.FileProviders
{
    public class CombinedFileProvider : IFileProvider
    {
        private readonly ISet<IFileProvider> Providers;

        public CombinedFileProvider(ISet<IFileProvider> providers)
        {
            Providers = providers;
        }

        public void Dispose()
        {
            foreach (var i in Providers)
            {
                i.Dispose();
            }
        }

        public async Task<IEnumerable<string>> ListEntriesAsync()
        {
            var tasks = Providers.Select(d => d.ListEntriesAsync()).ToArray();
            var results = await Task.WhenAll(tasks);
            var output = results.SelectMany(d => d.ToArray()).OrderBy(d => d).ToArray();
            return output;
        }

        public async Task<Stream> GetFileStreamAsync(string path, FileAccess accessType)
        {
            foreach(var i in Providers)
            {
                var stream = await i.GetFileStreamAsync(path, accessType);
                if (stream != null)
                {
                    return stream;
                }
            }

            return null;
        }
    }
}
