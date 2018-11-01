using System.Linq;
using System.Text.RegularExpressions;
using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.Core.Directives
{
    public class MinifyCodeDirective : IDirective
    {
        private static readonly Regex MinifyCode = new Regex(@"\s\s+");
        private CustomStory customStory;

        public void Execute(CustomStory customStory)
        {
            this.customStory = customStory;

            for(var i = 0; i < customStory.ProcessedFiles.Keys.Count; i++)
            {
                var processedFileValue = customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)];
                customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)] = GetMinifiedCode(processedFileValue);
            }
        }

        public string GetMinifiedCode(string code)
        {
            var minified = code;
            var matches = MinifyCode.Matches(code);
            minified = MinifyCode.Replace(minified, " ");
            minified = Regex.Replace(minified, "\n", "");
            minified = Regex.Replace(minified, "\r", "");
            return minified;
        }
    }
}
