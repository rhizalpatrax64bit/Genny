using Dnx.Genny.Scaffolding;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dnx.Genny.Templating
{
    public abstract class GennyModuleBase : IGennyModule
    {
        private String ModuleRoot { get; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected IApplicationEnvironment Environment { get; set; }

        public GennyModuleBase(IApplicationEnvironment environment, IGennyScaffolder scaffolder)
        {
            Scaffolder = scaffolder;
            Environment = environment;
            ModuleRoot = GetModuleRoot();
        }

        public virtual void Run()
        {
            Run(this);
        }
        protected virtual void Run(Object model)
        {
            if (ModuleRoot == null) return;

            String[] templates = Directory.GetFiles(ModuleRoot, "*.cshtml", SearchOption.AllDirectories);
            Dictionary<String, ScaffoldingResult> results = Scaffold(templates, model);

            Console.WriteLine();
            if (results.Any(result => result.Value.Errors.Any()))
                ConsoleWriteLine(ConsoleColor.Red, "Scaffolding failed! Rolling back...");
            else
                Write(results);
        }

        private Dictionary<String, ScaffoldingResult> Scaffold(IEnumerable<String> templates, Object model)
        {
            Dictionary<String, ScaffoldingResult> results = new Dictionary<String, ScaffoldingResult>();
            foreach (String template in templates)
            {
                Console.Write(template);
                ScaffoldingResult result = Scaffolder.Scaffold(File.ReadAllText(template), model);
                if (result.Errors.Any())
                {
                    ConsoleWriteLine(ConsoleColor.Red, " - Failed");
                    foreach (String error in result.Errors)
                        Console.WriteLine($"  - {error}");
                }
                else
                {
                    ConsoleWriteLine(ConsoleColor.Green, " - Succeeded");
                }

                results.Add(template, result);
            }

            return results;
        }
        private void Write(Dictionary<String, ScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, ScaffoldingResult> result in results)
            {
                String templatePath = Environment.ApplicationBasePath + result.Key.Replace(ModuleRoot, "");
                templatePath = templatePath.Remove(templatePath.Length - 7);

                if (!File.Exists(templatePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
                    File.WriteAllText(templatePath, result.Value.Content);
                }
                else
                {
                    ConsoleWriteLine(ConsoleColor.Yellow, $"{templatePath} already exists, skipping!");
                }
            }

            ConsoleWriteLine(ConsoleColor.Green, "Scaffolded successfully!");
        }

        private void ConsoleWriteLine(ConsoleColor color, String text)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(text);

            Console.ForegroundColor = currentColor;
        }
        private String GetModuleRoot()
        {
            String appRoot = Environment.ApplicationBasePath;
            String module = GetType().Name + ".cs";

            String[] files = Directory.GetFiles(appRoot, module, SearchOption.AllDirectories);
            if (files.Length != 1) return null;

            return Path.GetDirectoryName(files[0]);
        }
    }
}
