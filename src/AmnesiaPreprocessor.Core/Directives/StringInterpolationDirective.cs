using System.Linq;
using System.Text.RegularExpressions;
using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.Core.Directives
{
    public class StringInterpolationDirective : IDirective
    {
        private static readonly Regex StringInterpolation = new Regex(@"\$("".*?"")");

        private static readonly Regex InterpolatedString = new Regex(@"{(.*?)}");
        private CustomStory customStory;

        public void Execute(CustomStory customStory)
        {
            this.customStory = customStory;

            for(var i = 0; i < customStory.ProcessedFiles.Keys.Count; i++)
            {
                var processedFileValue = customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)];
                customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)] = GetConvertedIntrpolationCode(processedFileValue);
            }
        }

        public string GetConvertedIntrpolationCode(string code)
        {
            var result = code;
            var matches = StringInterpolation.Matches(code);

            foreach (Match match in matches)
            {
                var replacement = GetInterpolationReplacement(match.Groups[1].Value);
                result = result.Replace(match.Groups[0].Value, replacement);
            }
            
            return result;
        }

        private string GetInterpolationReplacement(string interpolationContents)
        {
            var result = interpolationContents;
            var matches = InterpolatedString.Matches(interpolationContents);

            foreach (Match match in matches)
            {
                result = result.Replace(match.Groups[0].Value, $"\" + {match.Groups[1].Value} + \"");
            }

            return result;
        }
    }
}
