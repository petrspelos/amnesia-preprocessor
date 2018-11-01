using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.Core.Directives
{
    public class GeneratePreloadsDirective : IDirective
    {
        private static readonly Regex SntStringRegex = new Regex(@"""([^\""]*\.snt)""");
        private static readonly Regex PsStringRegex = new Regex(@"""([^\""]*\.ps)""");
        private static readonly Regex OnEnterRegex = new Regex(@"void[\n\r\s]+?OnEnter[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{");
        
        private CustomStory customStory;

        public void Execute(CustomStory customStory)
        {
            this.customStory = customStory;

            for(var i = 0; i < customStory.ProcessedFiles.Keys.Count; i++)
            {
                var processedFileValue = customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)];
                customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)] = GetPreloadGeneratedCode(processedFileValue);
            }
        }

        public string GetPreloadGeneratedCode(string code)
        {
            if (code.Contains("void Preload()"))
            {
                Console.WriteLine("A Preload function is already defined and won't be generated...");
                return code;
            }

            var soundsToPreload = new List<string>();
            var particlesToPreload = new List<string>();

            foreach (Match match in SntStringRegex.Matches(code))
            {
                if (soundsToPreload.Contains(match.Groups[1].Value)) continue;
                soundsToPreload.Add(match.Groups[1].Value);
            }

            foreach (Match match in PsStringRegex.Matches(code))
            {
                if (particlesToPreload.Contains(match.Groups[1].Value)) continue;
                particlesToPreload.Add(match.Groups[1].Value);
            }

            var resultSb = new StringBuilder()
                .AppendLine("void Preload()")
                .AppendLine("{");

            foreach (var sound in soundsToPreload)
            {
                resultSb.AppendLine($"PreloadSound(\"{sound}\");");
            }

            foreach (var particle in particlesToPreload)
            {
                resultSb.AppendLine($"PreloadParticleSystem(\"{particle}\");");
            }

            resultSb.AppendLine("}")
                .AppendLine(code);

            if (!code.Contains("Preload();"))
            {
                if(OnEnterRegex.IsMatch(code))
                {
                    var newCode = OnEnterRegex.Replace(resultSb.ToString(), "void OnEnter() { Preload(); ");
                    resultSb = new StringBuilder(newCode);
                }
                else
                {
                    resultSb.Append("void OnEnter() { Preload(); }");
                }
            }

            return resultSb.ToString();
        }
    }
}
