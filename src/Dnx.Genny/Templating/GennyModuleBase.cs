using Dnx.Genny.Scaffolding;
using Microsoft.Framework.Runtime;
using System;
using System.IO;
using System.Linq;

namespace Dnx.Genny.Templating
{
    public abstract class GennyModuleBase : IGennyModule
    {
        protected IGennyScaffolder Scaffolder { get; set; }
        protected IApplicationEnvironment Environment { get; set; }

        public GennyModuleBase(IApplicationEnvironment environment, IGennyScaffolder scaffolder)
        {
            Environment = environment;
            Scaffolder = scaffolder;
        }

        public virtual void Run()
        {
            Run(this);
        }
        protected void Run(dynamic model)
        {
            String moduleRoot = GetModuleRoot();
            if (moduleRoot == null) return;

            String[] templates = Directory.GetFiles(moduleRoot, "*.cshtml", SearchOption.AllDirectories);

            foreach (String template in templates)
            {
                String templatePath = Environment.ApplicationBasePath + template.Replace(moduleRoot, "");
                ScaffoldingResult result = Scaffolder.Scaffold(File.ReadAllText(template), model);
                templatePath = templatePath.Remove(templatePath.Length - 7);

                if (result.Errors.Any())
                {
                    Console.WriteLine("Failed to scaffold:");
                    foreach (String error in result.Errors)
                        Console.WriteLine("  - " + error);
                }
                else
                {
                    if (!File.Exists(templatePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
                        File.WriteAllText(templatePath, result.Content);
                    }
                    else
                    {
                        Console.WriteLine($"{templatePath} already exists, skipping!");
                    }
                }
            }
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
