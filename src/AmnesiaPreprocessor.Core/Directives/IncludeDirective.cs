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
        private List<string> IgnoredIncludes = new List<string> { "AmnesiaSignatures.cpp" };
        private CustomStory customStory;

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

            return StripIncludeDirectives(dependencySource);
        }
    }
}
