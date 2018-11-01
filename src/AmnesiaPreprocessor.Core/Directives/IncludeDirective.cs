using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AmnesiaPreprocessor.Core.Entities;
using System.Text;
using System.IO;

namespace AmnesiaPreprocessor.Core.Directives
{
    public class IncludeDirective : IDirective
    {
        private readonly Regex IncludeStatementRegex = new Regex(@"#include ""(.*?)""(\n|\r)");
        private readonly Regex OnEnterContentRegex = new Regex(@"void[\n\r\s]+?OnEnter[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{(([^{]|\{.*?\})*?)}", RegexOptions.Singleline);
        private readonly Regex OnStartContentRegex = new Regex(@"void[\n\r\s]+?OnStart[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{(([^{]|\{.*?\})*?)}", RegexOptions.Singleline);
        private readonly Regex OnLeaveContentRegex = new Regex(@"void[\n\r\s]+?OnLeave[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{(([^{]|\{.*?\})*?)}", RegexOptions.Singleline);
        private static readonly Regex OnEnterRegex = new Regex(@"void[\n\r\s]+?OnEnter[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{");
        private static readonly Regex OnStartRegex = new Regex(@"void[\n\r\s]+?OnStart[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{");
        private static readonly Regex OnLeaveRegex = new Regex(@"void[\n\r\s]+?OnLeave[\n\r\s]*?\([\n\r\s]*?\)[\n\r\s]*?{");
        private List<string> IgnoredIncludes = new List<string> { "AmnesiaSignatures.cpp" };
        private CustomStory customStory;
        private string OnStartMergeRequest = "";
        private string OnEnterMergeRequest = "";
        private string OnLeaveMergeRequest = "";

        public void Execute(CustomStory customStory)
        {
            this.customStory = customStory;

            for(var i = 0; i < customStory.ProcessedFiles.Keys.Count; i++)
            {
                var processedFileValue = customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)];
                customStory.ProcessedFiles[customStory.ProcessedFiles.Keys.ElementAt(i)] = ProcessSourceIncludes(processedFileValue);
            }
        }

        private string ProcessSourceIncludes(string sourceCode)
        {
            var dependencies = GetSourceCodeDependencies(sourceCode);
            sourceCode = StripIncludeDirectives(sourceCode);
            sourceCode = AppendFoundDependencies(sourceCode, dependencies);
            sourceCode = MergeAndResetRequests(sourceCode);
            return sourceCode;
        }

        private IEnumerable<string> GetSourceCodeDependencies(string sourceCode)
        {
            // We have to use a foreach loop here because
            // Regex Matches returns a "MatchCollection"
            // object instead of an IEnumerable that we
            // could use Linq on.

            var dependencies = new List<string>();
            foreach(Match match in IncludeStatementRegex.Matches(sourceCode))
            {
                if(IgnoredIncludes.Contains(match.Groups[1].Value)) { continue; }
                dependencies.Add(match.Groups[1].Value);
            }
            return dependencies;
        }

        private string StripIncludeDirectives(string sourceCode)
        {
            return IncludeStatementRegex.Replace(sourceCode, string.Empty);
        }

        private string AppendFoundDependencies(string sourceCode, IEnumerable<string> dependencies)
        {
            var sourceCodeSb = new StringBuilder(sourceCode);
            
            foreach(var dependencyFile in dependencies)
            {
                var dependencySourceFile = customStory.IncludeHpsFiles.FirstOrDefault(f => f.EndsWith(dependencyFile));
                if(dependencySourceFile is null) { continue; }
                sourceCodeSb.Append(GetProcessedDependencyContents(dependencySourceFile, dependencies));
            }

            return sourceCodeSb.ToString();
        }

        // If "xxx.ihps" file has include directives, these must be in the source file's dependencies list.
        // Example:
        // If "A.shps" depends on "X.ihps" and "X.ihps" has a "include Y.ihps" in it, then "A.shps" must
        // also have that include directive.
        private string GetProcessedDependencyContents(string dependencyFile, IEnumerable<string> dependencies)
        {
            var dependencySource = File.ReadAllText(dependencyFile);
            var requiredDeps = GetSourceCodeDependencies(dependencySource);

            foreach(var requiredDep in requiredDeps)
            {
                if(!dependencies.Contains(requiredDep))
                {
                    throw new FileLoadException($"There is a missing transitive include directive for '{requiredDep}'.");
                }
            }

            dependencySource = StripIncludeDirectives(dependencySource);

            return SetMergeRequestsAndStripThem(dependencySource);
        }

        private string SetMergeRequestsAndStripThem(string sourceCode)
        {
            if(OnEnterContentRegex.IsMatch(sourceCode))
            {
                OnEnterMergeRequest = OnEnterContentRegex.Match(sourceCode).Groups[1].Value;
                sourceCode = OnEnterContentRegex.Replace(sourceCode, "");
            }

            if(OnStartContentRegex.IsMatch(sourceCode))
            {
                OnStartMergeRequest = OnStartContentRegex.Match(sourceCode).Groups[1].Value;
                sourceCode = OnStartContentRegex.Replace(sourceCode, "");
            }

            if(OnLeaveContentRegex.IsMatch(sourceCode))
            {
                OnLeaveMergeRequest = OnLeaveContentRegex.Match(sourceCode).Groups[1].Value;
                sourceCode = OnLeaveContentRegex.Replace(sourceCode, "");
            }

            return sourceCode;
        }

        private string MergeAndResetRequests(string sourceCode)
        {
            var resultSb = new StringBuilder(sourceCode);
            
            // Merge OnEnter
            if(!string.IsNullOrEmpty(OnEnterMergeRequest))
            {
                if(OnEnterRegex.IsMatch(sourceCode))
                {
                    var newCode = OnEnterRegex.Replace(resultSb.ToString(), $"void OnEnter() {{ {OnEnterMergeRequest}");
                    resultSb = new StringBuilder(newCode);
                }
                else
                {
                    resultSb.Append($"void OnEnter() {{ {OnEnterMergeRequest} }}");
                }
                
                OnEnterMergeRequest = "";
            }


            // Merge OnStart
            if(!string.IsNullOrEmpty(OnStartMergeRequest))
            {
                if(OnStartRegex.IsMatch(sourceCode))
                {
                    var newCode = OnStartRegex.Replace(resultSb.ToString(), $"void OnStart() {{ {OnStartMergeRequest}");
                    resultSb = new StringBuilder(newCode);
                }
                else
                {
                    resultSb.Append($"void OnStart() {{ {OnStartMergeRequest} }}");
                }
                
                OnStartMergeRequest = "";
            }


            // Merge OnLeave
            if(!string.IsNullOrEmpty(OnLeaveMergeRequest))
            {
                if(OnLeaveRegex.IsMatch(sourceCode))
                {
                    var newCode = OnLeaveRegex.Replace(resultSb.ToString(), $"void OnLeave() {{ {OnLeaveMergeRequest}");
                    resultSb = new StringBuilder(newCode);
                }
                else
                {
                    resultSb.Append($"void OnLeave() {{ {OnLeaveMergeRequest} }}");
                }
                
                OnLeaveMergeRequest = "";
            }

            return resultSb.ToString();
        }
    }
}
