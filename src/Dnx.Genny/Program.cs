using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnx.Genny
{
    public class Program
    {
        private IGennyLogger Logger { get; }
        private IGennyModuleLoader Loader { get; }
        private IGennyModuleLocator Locator { get; }
        private GennyServiceProvider ServiceProvider { get; }

        public Program(IServiceProvider serviceProvider)
        {
            ServiceProvider = new GennyServiceProvider(serviceProvider);

            ServiceProvider.Add<IGennyLogger, GennyLogger>();
            ServiceProvider.Add<IGennyCompiler, GennyCompiler>();
            ServiceProvider.Add<IGennyScaffolder, GennyScaffolder>();
            ServiceProvider.Add<IGennyModuleLoader, GennyModuleLoader>();
            ServiceProvider.Add<IGennyModuleLocator, GennyModuleLocator>();

            Logger = ServiceProvider.GetRequiredService<IGennyLogger>();
            Loader = ServiceProvider.GetRequiredService<IGennyModuleLoader>();
            Locator = ServiceProvider.GetRequiredService<IGennyModuleLocator>();
        }

        public void Main(String[] args)
        {
            if (ShowHelpFor(args))
            {
                ShowHelp();
                ShowAvailableModules();
            }
            else
            {
                String moduleName = args[0];
                String[] moduleArgs = args.Skip(1).ToArray();
                GennyModuleDescriptor[] descriptors = Locator.Find(moduleName).ToArray();

                switch(descriptors.Count())
                {
                    case 0:
                        Logger.Write($"Could not find a genny module named: {moduleName}");
                        ShowAvailableModules();

                        break;
                    case 1:
                        GennyModuleLoaderResult result = Loader.Load(descriptors[0], moduleArgs);
                        if (result.Errors.Any())
                        {
                            foreach (String error in result.Errors)
                                Logger.Write(error);

                            result.Module.ShowHelp(Logger);
                        }
                        else
                        {
                            result.Module.Run();
                        }

                        break;
                    default:
                        Logger.Write($"Found more than one genny module named: {moduleName}");
                        ShowAvailableModules();

                        break;
                }
            }
        }

        private Boolean ShowHelpFor(String[] args)
        {
            return args.Length == 0 || new[] { "-?", "-h", "--help" }.Contains(args.FirstOrDefault());
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
