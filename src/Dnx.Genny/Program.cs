using Dnx.Genny.Compilation;
using Dnx.Genny.Scaffolding;
using Dnx.Genny.Services;
using Dnx.Genny.Templating;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnx.Genny
{
    public class Program
    {
        private ServiceProvider ServiceProvider { get; }

        public Program(IServiceProvider serviceProvider)
        {
            ServiceProvider = new ServiceProvider(serviceProvider);

            ServiceProvider.Add<IGennyModuleLocator, GennyModuleLocator>();
            ServiceProvider.Add<IGennyCompiler, GennyCompiler>();
            ServiceProvider.Add<IScaffolder, Scaffolder>();
        }

        public void Main(String[] args)
        {
            if (args == null || args.Length == 0 || IsHelpArgument(args[0]))
            {
                ShowHelp();
            }
            else
            {
                IGennyModuleLocator locator = ServiceProvider.GetService<IGennyModuleLocator>();
                IEnumerable<GennyModuleDescription> descriptions = locator.Find(args[0]);
                GennyModuleDescription module = descriptions.FirstOrDefault();
                Int32 descriptionCount = descriptions.Count();

                switch(descriptionCount)
                {
                    case 0:
                        Console.WriteLine($"Could not find a genny module named: {args[0] + Environment.NewLine}");
                        ShowAvailableModules();
                        break;
                    case 1:
                        IGennyModule gennyModule = ActivatorUtilities.CreateInstance(ServiceProvider, module.Type) as IGennyModule;
                        Console.WriteLine($"Running genny module: {module.Name + Environment.NewLine}");

                        gennyModule.Scaffolder = ServiceProvider.GetService<IScaffolder>();
                        gennyModule.Run();
                        break;
                    default:
                        Console.WriteLine($"Found more than one genny modules named: {args[0] + Environment.NewLine}");
                        ShowAvailableModules();
                        break;
                }
            }
        }

        private void ShowAvailableModules()
        {
            IGennyModuleLocator locator = ServiceProvider.GetService<IGennyModuleLocator>();
            IEnumerable<GennyModuleDescription> descriptions = locator.FindAll();

            if (descriptions.Any())
            {
                Console.WriteLine("Available genny modules:");
                foreach (GennyModuleDescription description in descriptions)
                    Console.WriteLine("    " + description.Name + " - " + (description.Comments ?? "{No description}"));
            }
            else
            {
                Console.WriteLine("There are no genny modules installed!");
            }
        }

        private Boolean IsHelpArgument(String arg)
        {
            return new[] { "?", "-?", "-h", "-help", "--?", "--h", "--help" }
                .Any(helpArg => String.Equals(helpArg, arg, StringComparison.OrdinalIgnoreCase));
        }
        private void ShowHelp()
        {
            Console.WriteLine("Usage: dnx . <genny command name> <genny module name>");
            ShowAvailableModules();
        }
    }
}
