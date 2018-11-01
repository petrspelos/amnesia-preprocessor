using System.Linq;
using System.Text.RegularExpressions;
using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.Core.Directives
{
    public class RemoveCommentsDirective : IDirective
    {
        private static readonly Regex CodeCommentsRegex = new Regex(@"(\/\*(.|\n)*?\*\/)|(\/\/(.*?)\r?\n)");
        private CustomStory customStory;

        public void Execute(CustomStory customStory)
        {
            this.customStory = customStory;

            for(var i = 0; i < customStory.ProcessedFiles.Keys.Count; i++)
            {
                var processedFileValue = customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)];
                customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)] = GetCommentlessCode(processedFileValue);
            }
        }

        public string GetCommentlessCode(string code)
        {
            return CodeCommentsRegex.Replace(code, "");
        }
    }
}
