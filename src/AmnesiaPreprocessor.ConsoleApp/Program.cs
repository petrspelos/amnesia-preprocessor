using System;
using System.IO;
using System.Linq;
using AmnesiaPreprocessor.Core.Directives;
using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!args.Any(a => a.StartsWith("-cs:")))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You must provide the '-cs:' argument with the custom story path.");
                Console.ResetColor();
                return;
            }

            var csPath = args.First(a => a.StartsWith("-cs:")).Substring(4);

            if(!Directory.Exists($@"{csPath}\maps"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($@"The path: '{csPath}\maps' does not exist!");
                Console.ResetColor();
                return;
            }

            var doNotMinify = args.Any(a => a == "--no-min");

            var cs = new CustomStory(csPath);
            
            IDirective includeDirective = new IncludeDirective();
            IDirective stringInterpolationDirective = new StringInterpolationDirective();
            IDirective generatePreloadsDirective = new GeneratePreloadsDirective();
            IDirective removeCommentsDirective = new RemoveCommentsDirective();
            IDirective minifyCodeDirective = new MinifyCodeDirective();

            try
            {
                includeDirective.Execute(cs);
                stringInterpolationDirective.Execute(cs);
                generatePreloadsDirective.Execute(cs);
                removeCommentsDirective.Execute(cs);
                
                if(!doNotMinify)
                {
                    minifyCodeDirective.Execute(cs);
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Preprocessing failed!");
                Console.WriteLine($"Reason: {e.Message}");
                Console.ResetColor();
                return;
            }

            foreach(var compiledFile in cs.ProcessedFiles)
            {
                File.WriteAllText($@"{csPath}\maps\{compiledFile.Key}", compiledFile.Value);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Preprocessing done!");
            Console.ResetColor();
        }
    }
}
