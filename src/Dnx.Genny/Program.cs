using Dnx.Genny.Compilation;
using Dnx.Genny.Scaffolding;
using Dnx.Genny.Services;
using Microsoft.Framework.DependencyInjection;
using System;
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
            ServiceProvider.Add<IScaffolder, Scaffolder>();
        }

        public void Main(String[] args)
        {
            IScaffolder scaffolder = ServiceProvider.GetService<IScaffolder>();
            ScaffoldingResult result = scaffolder.Scaffold(@"Scaffolded: @Model", args.Length > 0 ? args[0] : "");

            if (result.Errors.Any())
            {
                Console.WriteLine("Failed to scaffold:");
                foreach (String error in result.Errors)
                    Console.WriteLine("  - " + error);
            }
            else
            {
                Console.WriteLine(result.Content);
            }
        }
    }
}
