using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Genny
{
    public abstract class GennyModule : IGennyModule
    {
        protected IGennyLogger Logger { get; set; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected GennyApplication Application { get; set; }

        protected GennyModule(IServiceProvider provider)
        {
            Application = provider.GetRequiredService<GennyApplication>();
            Scaffolder = provider.GetService<IGennyScaffolder>();
            Logger = provider.GetService<IGennyLogger>();
            Scaffolder.ModuleRootPath = FindModuleRoot();
        }

        private String FindModuleRoot()
        {
            String[] files = Directory.GetFiles(Application.BasePath, GetType().Name + ".cs", SearchOption.AllDirectories);

            return files.Length == 1 ? Path.GetDirectoryName(files[0]) : null;
        }
        public abstract void Run();
        public virtual void ShowHelp()
        {
            Logger.WriteLine("    Help is not available for this module.");
        }

        protected virtual void Write(String path, GennyScaffoldingResult result)
        {
            if (result.Errors.Any())
            {
                Logger.Write($"{path} - ");
                Logger.WriteLine("Failed", ConsoleColor.Red);

                foreach (String error in result.Errors)
                    Logger.WriteLine($"  - {error}");
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, result.Content);

                Logger.Write($"{path} - ");
                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        protected virtual void Write(IDictionary<String, GennyScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, GennyScaffoldingResult> result in results)
                Write(result.Key, result.Value);
        }

        protected virtual void TryWrite(String path, GennyScaffoldingResult result)
        {
            if (File.Exists(path))
            {
                Logger.Write($"{path} - ");
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                Write(path, result);
            }
        }
        protected virtual void TryWrite(IDictionary<String, GennyScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, GennyScaffoldingResult> result in results)
                TryWrite(result.Key, result.Value);
        }
    }
}
