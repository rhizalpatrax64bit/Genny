using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genny
{
    public class GennyCommand
    {
        private IGennyLogger Logger { get; }
        private IGennyModuleLoader Loader { get; }
        private IGennyModuleLocator Locator { get; }
        private GennyServiceProvider ServiceProvider { get; }

        public GennyCommand()
        {
            ServiceProvider = new GennyServiceProvider();

            ServiceProvider.Add(new GennyApplication());
            ServiceProvider.Add<IGennyLogger, GennyLogger>();
            ServiceProvider.Add<IGennyCompiler, GennyCompiler>();
            ServiceProvider.Add<IGennyScaffolder, GennyScaffolder>();
            ServiceProvider.Add<IGennyModuleLoader, GennyModuleLoader>();
            ServiceProvider.Add<IGennyModuleLocator, GennyModuleLocator>();

            Logger = ServiceProvider.GetRequiredService<IGennyLogger>();
            Loader = ServiceProvider.GetRequiredService<IGennyModuleLoader>();
            Locator = ServiceProvider.GetRequiredService<IGennyModuleLocator>();
        }

        public void Execute(String[] args)
        {
            if (args.Length == 0 || ShowHelpFor(args))
            {
                ShowHelp();
                ShowAvailableModules();
            }
            else
            {
                String moduleName = args[0];
                String[] moduleArgs = args.Skip(1).ToArray();
                GennyModuleDescriptor[] descriptors = Locator.Find(moduleName).ToArray();

                switch (descriptors.Length)
                {
                    case 0:
                        Logger.WriteLine($"Could not find a genny module named: {moduleName}", ConsoleColor.Red);
                        ShowAvailableModules();

                        break;
                    case 1:
                        GennyModuleLoaderResult result = Loader.Load(descriptors[0], moduleArgs);

                        if (ShowHelpFor(moduleArgs))
                        {
                            result.Module.ShowHelp();
                        }
                        else if (result.Errors.Any())
                        {
                            foreach (String error in result.Errors)
                                Logger.WriteLine(error, ConsoleColor.Red);

                            result.Module.ShowHelp();
                        }
                        else
                        {
                            result.Module.Run();
                        }

                        break;
                    default:
                        Logger.WriteLine($"Found more than one genny module named: {moduleName}, try using longer identifiers");
                        foreach (GennyModuleDescriptor descriptor in descriptors)
                            Logger.WriteLine($"    {descriptor.FullName} - {descriptor.Description ?? "{No description}"}");

                        break;
                }
            }
        }

        private Boolean ShowHelpFor(String[] args)
        {
            return args.Length == 1 && new[] { "-?", "-h", "--help" }.Contains(args[0]);
        }
        private void ShowAvailableModules()
        {
            IEnumerable<GennyModuleDescriptor> descriptors = Locator.FindAll();

            if (descriptors.Any())
            {
                Logger.WriteLine("Available genny modules:");
                foreach (GennyModuleDescriptor descriptor in descriptors)
                    Logger.WriteLine($"    {descriptor.Name} - {descriptor.Description ?? "{No description}"}");
            }
            else
            {
                Logger.WriteLine("There are no genny modules installed!", ConsoleColor.Red);
            }
        }
        private void ShowHelp()
        {
            Logger.WriteLine("Usage: dotnet gen <module name> [module parameters]");
        }
    }
}
