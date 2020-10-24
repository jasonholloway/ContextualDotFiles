using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContextualDotFiles
{
    public abstract class DotFileLoader<TFile> : IDotFileLoader<TFile>
    {
        protected abstract Regex FileNameRegex { get; }
        protected abstract TFile Zero { get; }
        protected abstract TFile Combine(TFile left, TFile right);
        protected abstract Task<TFile> ReadFile(Stream stream);
        
        public async Task<TFile> Load(DirectoryInfo rootDir)
        {
            var streams = DotFileFinder
                .FindAll(rootDir, FileNameRegex)
                .Select(f => f.OpenRead())
                .ToArray();

            try
            {
                var files = await Task.WhenAll(
                    streams.Select(ReadFile));

                return files.Aggregate(Zero, Combine);
            }
            finally
            {
                foreach (var stream in streams)
                {
                    stream.Dispose();
                }
            }
        }
    }
}