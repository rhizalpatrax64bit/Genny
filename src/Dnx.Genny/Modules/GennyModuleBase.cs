using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dnx.Genny
{
    public abstract class GennyModuleBase : IGennyModule
    {
        protected String ModuleRoot { get; }
        protected IGennyLogger Logger { get; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected IApplicationEnvironment Environment { get; set; }

        protected GennyModuleBase(IServiceProvider provider)
        {
            Environment = provider.GetService<IApplicationEnvironment>();
            Scaffolder = provider.GetService<IGennyScaffolder>();
            Logger = provider.GetService<IGennyLogger>();
            ModuleRoot = GetModuleRoot();
        }

        public virtual void Run()
        {
            Run(this);
        }
        public virtual void ShowHelp(IGennyLogger logger)
        {
            logger.Write("    Help is not available for this module.");
        }
 
        protected virtual void Run(Object model)
        {
            if (ModuleRoot == null)
            {
                Logger.Write("Module root folder was not found, aborting...");

                return;
            }

            String[] templates = Directory.GetFiles(ModuleRoot, "*.cshtml", SearchOption.AllDirectories);
            List<ScaffoldingResult> results = new List<ScaffoldingResult>(templates.Length);

            foreach (String template in templates)
            {
                String outputPath = template.Remove(template.Length - 7).Replace(ModuleRoot, "").TrimStart('\\', '/');
                String shortPath = Path.Combine(Environment.ApplicationName, outputPath);

                ScaffoldingResult result = Scaffolder.Scaffold(template, Environment.ApplicationName, outputPath, model);
                if (result.Errors.Any())
                {
                    Logger.Write($"{shortPath} - Failed");
                    foreach (String error in result.Errors)
                        Logger.Write($"  - {error}");
                }
                else
                {
                    if (!File.Exists(result.Path))
                        Logger.Write($"{shortPath} - Succeeded");
                    else
                        Logger.Write($"{shortPath} - Already exists, skipping...");
                }

                results.Add(result);
            }

            if (results.Any(result => result.Errors.Any()))
            {
                Logger.Write("Scaffolding failed! Rolling back...");
            }
            else
            {
                Write(results);
                Logger.Write("Scaffolded successfully!");
            }
        }
        protected virtual void Write(ScaffoldingResult result)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(result.Path));
            File.WriteAllText(result.Path, result.Content);
        }
        protected virtual void Write(IEnumerable<ScaffoldingResult> results)
        {
            foreach (ScaffoldingResult result in results)
                Write(result);
        }

        private String GetModuleRoot()
        {
            String[] files = Directory.GetFiles(Environment.ApplicationBasePath, GetType().Name + ".cs", SearchOption.AllDirectories);
            if (files.Length != 1) return null;

            return Path.GetDirectoryName(files[0]);
        }
    }
}
