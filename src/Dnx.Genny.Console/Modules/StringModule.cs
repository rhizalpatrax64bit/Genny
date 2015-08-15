using Dnx.Genny.Scaffolding;
using Dnx.Genny.Templating;
using System;
using System.Linq;

namespace Dnx.Genny.Console.Modules
{
    public class StringModule : IGennyModule
    {
        public IScaffolder Scaffolder { get; set; }

        public void Run()
        {
            ScaffoldingResult result = Scaffolder.Scaffold(@"My razor content: @Model", "o/");

            if (result.Errors.Any())
            {
                System.Console.WriteLine("Failed to scaffold:");
                foreach (String error in result.Errors)
                    System.Console.WriteLine("  - " + error);
            }
            else
            {
                System.Console.WriteLine(result.Content);
            }
        }
    }
}
