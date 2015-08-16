using Microsoft.CodeAnalysis;
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

            ServiceProvider.Add<IGennyCompiler, GennyCompiler>();
            ServiceProvider.Add<IGennyScaffolder, GennyScaffolder>();
            ServiceProvider.Add<IGennyModuleLocator, GennyModuleLocator>();
        }

        public void Main(String[] args)
        {
            if (ShouldShowHelp(args))
            {
                ShowHelp();
                ShowAvailableModules();
            }
            else
            {
                IGennyModuleLocator locator = ServiceProvider.GetService<IGennyModuleLocator>();
                IEnumerable<GennyModuleDescriptor> descriptors = locator.Find(args[0]);
                GennyModuleDescriptor descriptor = descriptors.FirstOrDefault();
                Int32 descriptorCount = descriptors.Count();

                switch(descriptorCount)
                {
                    case 0:
                        Console.WriteLine($"Could not find a genny module named: {args[0] + Environment.NewLine}");
                        ShowAvailableModules();

                        break;
                    case 1:
                        IGennyModule module = ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type) as IGennyModule;
                        GennyCommandLineParser parser = new GennyCommandLineParser();
                        parser.ParseTo(module, args.Skip(1).ToArray());

                        Console.WriteLine($"Scaffolding genny module: {descriptor.Name + Environment.NewLine}");
                        module.Run();

                        break;
                    default:
                        Console.WriteLine($"Found more than one genny module named: {args[0] + Environment.NewLine}");
                        ShowAvailableModules();

                        break;
                }
            }
        }

        private Boolean ShouldShowHelp(String[] args)
        {
            return args == null || args.Length == 0 ||
                new[] { "?", "-?", "-h", "-help", "--?", "--h", "--help" }
                    .Any(helpArg => String.Equals(args[0], helpArg, StringComparison.OrdinalIgnoreCase));
        }
        private void ShowAvailableModules()
        {
            IGennyModuleLocator locator = ServiceProvider.GetService<IGennyModuleLocator>();
            IEnumerable<GennyModuleDescriptor> descriptors = locator.FindAll();

            if (descriptors.Any())
            {
                Console.WriteLine("Available genny modules:");
                foreach (GennyModuleDescriptor descriptor in descriptors)
                    Console.WriteLine($"    {descriptor.Name} - {descriptor.Description ?? "{No description}"}");
            }
            else
            {
                Console.WriteLine("There are no genny modules installed!");
            }
        }
        private void ShowHelp()
        {
            Console.WriteLine("Usage: dnx . <genny command name> <genny module name>");
        }
    }
}
