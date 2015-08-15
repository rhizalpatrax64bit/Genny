using Dnx.Genny.Scaffolding;
using Dnx.Genny.Services;
using Microsoft.Framework.Runtime;
using System;
using System.IO;
using System.Linq;

namespace Dnx.Genny.Templating
{
    public abstract class GennyModuleBase : IGennyModule
    {
        public ServiceProvider ServiceProvider { get; set; }
        public IScaffolder Scaffolder { get; set; }

        public virtual void Run()
        {
            IApplicationEnvironment environment = ServiceProvider.GetService<IApplicationEnvironment>();
            String moduleRoot = GetModuleRoot(environment);
            if (moduleRoot == null) return;

            String[] templates = Directory.GetFiles(moduleRoot, "*.cshtml", SearchOption.AllDirectories);

            foreach (String template in templates)
            {
                String templatePath = environment.ApplicationBasePath + template.Replace(moduleRoot, "");
                ScaffoldingResult result = Scaffolder.Scaffold(File.ReadAllText(template));
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

        private String GetModuleRoot(IApplicationEnvironment environment)
        {
            String appRoot = environment.ApplicationBasePath;
            String module = GetType().Name + ".cs";

            String[] files = Directory.GetFiles(appRoot, module, SearchOption.AllDirectories);
            if (files.Length != 1) return null;

            return Path.GetDirectoryName(files[0]);
        }
    }
}
