Code scaffolder for DNX projects.

This project is currently in development.

### Usage
Locate the commands section in `project.json` and add the gen command:
```JSON
"commands": {
  "gen": "Dnx.Genny"
}
```

Implement `IGennyModule`:
```C#
using System;
using System.Linq;
using Dnx.Genny.Templating;
using Dnx.Genny.Scaffolding;

public class MyGennyModule : IGennyModule
{
    public IScaffolder Scaffolder { get; set; }

    public void Run()
    {
        ScaffoldingResult result = Scaffolder.Scaffold(@"My razor content: @Model", "o/");

        if (result.Errors.Any())
        {
            Console.WriteLine("Failed to scaffold:");
            foreach (String error in result.Errors)
                Console.WriteLine("  - " + error);
        }
        else
        {
            Console.WriteLine(result.Content);
            // Write content to file or any other source.
        }
    }
}
```

Run your module from command line, by navigating to the project directory and run:
`dnx . gen my-genny-module`