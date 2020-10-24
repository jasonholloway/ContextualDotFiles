using System.IO;
using System.Threading.Tasks;

namespace ContextualDotFiles
{
    public interface IDotFileLoader<TFile>
    {
        Task<TFile> Load(DirectoryInfo rootDir);
    }
}