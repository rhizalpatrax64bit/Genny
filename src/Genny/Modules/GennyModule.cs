using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Genny
{
    public abstract class GennyModule : IGennyModule
    {
        protected String ModuleRoot { get; set; }
        protected IGennyLogger Logger { get; set; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected GennyApplication Application { get; set; }

        protected GennyModule(IServiceProvider provider)
        {
            Application = provider.GetRequiredService<GennyApplication>();
            Scaffolder = provider.GetService<IGennyScaffolder>();
            Logger = provider.GetService<IGennyLogger>();
            ModuleRoot = FindModuleRoot();
        }
        private String FindModuleRoot()
        {
            String[] files = Directory.GetFiles(Application.BasePath, GetType().Name + ".cs", SearchOption.AllDirectories);
            if (files.Length != 1) return null;

            return Path.GetDirectoryName(files[0]);
        }

        public abstract void Run();
        public virtual void ShowHelp(IGennyLogger logger)
        {
            logger.Write("    Help is not available for this module.");
        }

        protected virtual String ReadTemplate(params String[] paths)
        {
            return File.ReadAllText(Path.Combine(paths));
        }

        protected virtual void Write(String path, GennyScaffoldingResult result)
        {
            if (result.Errors.Any())
            {
                Logger.Write($"{path} - Failed");
                foreach (String error in result.Errors)
                    Logger.Write($"  - {error}");
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, result.Content);
                Logger.Write($"{path} - Succeeded");
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
                Logger.Write($"{path} - Already exists, skipping...");
            else
                Write(path, result);
        }
        protected virtual void TryWrite(IDictionary<String, GennyScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, GennyScaffoldingResult> result in results)
                TryWrite(result.Key, result.Value);
        }
    }
}
