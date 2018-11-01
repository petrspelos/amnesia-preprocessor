using System.Collections.Generic;

namespace AmnesiaPreprocessor.Core.Entities
{
    public class CustomStory
    {
        public string RootPath { get; private set; }
        public List<string> SourceHpsFiles { get; private set; }
        public List<string> IncludeHpsFiles { get; private set; }

        public CustomStory(string rootPath)
        {
            RootPath = rootPath;
        }
    }
}
