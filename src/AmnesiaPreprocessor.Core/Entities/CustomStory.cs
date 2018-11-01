using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AmnesiaPreprocessor.Core.Entities
{
    public class CustomStory
    {
        public string RootPath { get; private set; }
        public ImmutableList<string> SourceHpsFiles { get; private set; }
        public ImmutableList<string> IncludeHpsFiles { get; private set; }
        public Dictionary<string, string> ProcessedFiles { get; set; }

        public CustomStory(string rootPath)
        {
            RootPath = rootPath;
            ThrowIfRootPathDoesNotExist();
            LoadCustomStoryFiles();
            InitializeProcessedFiles();
        }

        private void ThrowIfRootPathDoesNotExist()
        {
            if(!Directory.Exists(RootPath))
            { 
                throw new DirectoryNotFoundException();
            }
        }

        private void LoadCustomStoryFiles()
        {
            SourceHpsFiles = Directory.GetFiles(RootPath, "*.shps", SearchOption.AllDirectories).ToImmutableList();
            IncludeHpsFiles = Directory.GetFiles(RootPath, "*.ihps", SearchOption.AllDirectories).ToImmutableList();
        }

        private void InitializeProcessedFiles()
        {
            ProcessedFiles = new Dictionary<string, string>();
            foreach(var sourceFilePath in SourceHpsFiles)
            {
                ProcessedFiles.Add(Path.GetFileName(sourceFilePath), File.ReadAllText(sourceFilePath));
            }
        }
    }
}
