using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dnx.Genny
{
    public abstract class GennyModule : IGennyModule
    {
        protected String ModuleRoot { get; }
        protected IGennyLogger Logger { get; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected IApplicationEnvironment Environment { get; set; }

        protected GennyModule(IServiceProvider provider)
        {
            Environment = provider.GetRequiredService<IApplicationEnvironment>();
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
            IDictionary<String, ScaffoldingResult> results = new Dictionary<String, ScaffoldingResult>(templates.Length);

            foreach (String template in templates)
            {
                String outputPath = template.Remove(template.Length - 7).Replace(ModuleRoot, "").TrimStart('\\', '/');
                String resultPath = FormResultPath(Environment.ApplicationName, outputPath);
                String shortPath = Path.Combine(Environment.ApplicationName, outputPath);

                ScaffoldingResult result = Scaffolder.Scaffold(Read(template), model);
                if (result.Errors.Any())
                {
                    Logger.Write($"{shortPath} - Failed");
                    foreach (String error in result.Errors)
                        Logger.Write($"  - {error}");
                }
                else
                {
                    if (!File.Exists(resultPath))
                    {
                        results.Add(resultPath, result);
                        Logger.Write($"{shortPath} - Succeeded");
                    }
                    else
                    {
                        Logger.Write($"{shortPath} - Already exists, skipping...");
                    }
                }
            }

            if (results.Any(result => result.Value.Errors.Any()))
            {
                Logger.Write("Scaffolding failed! Rolling back...");
            }
            else
            {
                Write(results);
                Logger.Write("Scaffolded successfully!");
            }
        }
        protected virtual void Write(String path, ScaffoldingResult result)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, result.Content);
        }
        protected virtual void Write(IDictionary<String, ScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, ScaffoldingResult> result in results)
                Write(result.Key, result.Value);
        }

        private String FormResultPath(String project, String outputPath)
        {
            String path = Path.GetDirectoryName(Environment.ApplicationBasePath);
            path = Path.Combine(path, project, outputPath);
            path = Path.GetFullPath(path);

            return path;
        }
        private String Read(String template)
        {
            String path = Path.Combine(Environment.ApplicationBasePath, template);

            return File.ReadAllText(path);
        }
        private String GetModuleRoot()
        {
            String[] files = Directory.GetFiles(Environment.ApplicationBasePath, GetType().Name + ".cs", SearchOption.AllDirectories);
            if (files.Length != 1) return null;

            return Path.GetDirectoryName(files[0]);
        }
    }
}
