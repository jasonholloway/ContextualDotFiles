using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ContextualDotFiles.Tests
{
    public class Tests
    {
        [Test]
        public void FindsFiles()
        {
            var found = DotFileFinder.FindAll(
                new DirectoryInfo(RootPath("folder1", "folder2")), 
                new Regex(@"\.envfile"));
            
            Assert.That(found.Select(f => f.FullName), 
                Is.EqualTo(new[]
                {
                    RootPath("folder1", ".envfile"), 
                    RootPath("folder1", "folder2", ".envfile")
                }));
        }
        
        [Test]
        public void CopesWithNonExistentRoot()
        {
            var found = DotFileFinder.FindAll(
                new DirectoryInfo(RootPath("folder1", "blah")), 
                new Regex(@"\.envfile"));

            Assert.That(found, Is.EqualTo(new FileInfo[0]));
        }

        [Test]
        public async Task LoadsAndCombinesFiles()
        {
            var loader = new TestFileLoader();
            
            var loaded = await loader.Load(
                new DirectoryInfo(RootPath("folder1", "folder2")));
            
            Assert.That(loaded,
                Is.EqualTo(new[]
                {
                    "Well",
                    "hello",
                    "there!",
                }));
        }
        
        static string RootPath(params string[] parts)
            => Path.Combine(
                new[] { TestContext.CurrentContext.TestDirectory }.Concat(parts).ToArray()
            );

        class TestFileLoader : DotFileLoader<string[]>
        {
            protected override Regex FileNameRegex => new Regex(@"\.envfile");
            
            protected override string[] Zero => new string[0];

            protected override string[] Combine(string[] left, string[] right)
                => left.Concat(right).ToArray();

            protected override async Task<string[]> ReadFile(Stream stream)
            {
                using var reader = new StreamReader(stream);
                var data = await reader.ReadToEndAsync();
                return data.Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}