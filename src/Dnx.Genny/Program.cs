using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnx.Genny
{
    public class Program
    {
        private IGennyLogger Logger { get; }
        private ServiceProvider ServiceProvider { get; }

        public Program(IServiceProvider serviceProvider)
        {
            ServiceProvider = new ServiceProvider(serviceProvider);

            ServiceProvider.Add<IGennyLogger, GennyLogger>();
            ServiceProvider.Add<IGennyCompiler, GennyCompiler>();
            ServiceProvider.Add<IGennyScaffolder, GennyScaffolder>();
            ServiceProvider.Add<IGennyModuleLocator, GennyModuleLocator>();

            Logger = ServiceProvider.GetService<IGennyLogger>();
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
                        Logger.Write($"Could not find a genny module named: {args[0]}");
                        ShowAvailableModules();

                        break;
                    case 1:
                        IGennyModule module = ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type) as IGennyModule;
                        if (args.Length == 1 || ShouldShowHelp(args.Skip(1).ToArray()))
                        {
                            module.ShowHelp(Logger);
                        }
                        else
                        {
                            GennyCommandLineParser parser = new GennyCommandLineParser();
                            parser.ParseTo(module, args.Skip(1).ToArray());
                            module.Run();
                        }

                        break;
                    default:
                        Logger.Write($"Found more than one genny module named: {args[0]}");
                        ShowAvailableModules();

                        break;
                }
            }
        }

        private Boolean ShouldShowHelp(String[] args)
        {
            return args == null || args.Length == 0 ||
                new[] { "?", "-?", "-h", "--help" }.Any(arg => String.Equals(args[0], arg, StringComparison.OrdinalIgnoreCase));
        }
        private void ShowAvailableModules()
        {
            IGennyModuleLocator locator = ServiceProvider.GetService<IGennyModuleLocator>();
            IEnumerable<GennyModuleDescriptor> descriptors = locator.FindAll();

            if (descriptors.Any())
            {
                Logger.Write("Available genny modules:");
                foreach (GennyModuleDescriptor descriptor in descriptors)
                    Logger.Write($"    {descriptor.Name} - {descriptor.Description ?? "{No description}"}");
            }
            else
            {
                Logger.Write("There are no genny modules installed!");
            }
        }
        private void ShowHelp()
        {
            Logger.Write("Usage: dnx <genny command name> <genny module name> [genny module parameters]");
        }
    }
}
