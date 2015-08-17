﻿using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dnx.Genny
{
    public abstract class GennyModuleBase : IGennyModule
    {
        protected IApplicationEnvironment Environment { get; set; }
        protected IGennyScaffolder Scaffolder { get; set; }
        protected IGennyLogger Logger { get; }
        private String ModuleRoot { get; }

        public GennyModuleBase(IServiceProvider provider)
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
        protected virtual void Run(Object model)
        {
            if (ModuleRoot == null) return;

            String[] templates = Directory.GetFiles(ModuleRoot, "*.cshtml", SearchOption.AllDirectories);
            Dictionary<String, ScaffoldingResult> results = Scaffold(templates, model);

            if (results.Any(result => result.Value.Errors.Any()))
                Logger.Write("Scaffolding failed! Rolling back...");
            else
                Write(results);
        }

        private Dictionary<String, ScaffoldingResult> Scaffold(IEnumerable<String> templates, Object model)
        {
            Dictionary<String, ScaffoldingResult> results = new Dictionary<String, ScaffoldingResult>();
            foreach (String template in templates)
            {
                ScaffoldingResult result = Scaffolder.Scaffold(File.ReadAllText(template), model);
                String folderStart = template.Remove(template.Length - 7).Replace(ModuleRoot, "");
                String templatePath = Environment.ApplicationBasePath + folderStart;
                String projectPath = Environment.ApplicationName + folderStart;

                if (result.Errors.Any())
                {
                    Logger.Write($"{projectPath} - Failed");
                    foreach (String error in result.Errors)
                        Logger.Write($"  - {error}");
                }
                else
                {
                    if (!File.Exists(templatePath))
                        Logger.Write($"{projectPath} - Succeeded");
                    else
                        Logger.Write($"{projectPath} - Already exists, skipping");
                }

                results.Add(templatePath, result);
            }

            return results;
        }
        private void Write(Dictionary<String, ScaffoldingResult> results)
        {
            foreach (KeyValuePair<String, ScaffoldingResult> result in results)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(result.Key));
                File.WriteAllText(result.Key, result.Value.Content);
            }

            Logger.Write("Scaffolded successfully!");
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
