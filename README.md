Code generator for DotNet projects.

# Installation

Install genny dependencies:

```XML
<ItemGroup>
  <PackageReference Include="Genny" Version="<version>" />
</ItemGroup>

<ItemGroup>
  <DotNetCliToolReference Include="Genny" Version="<version>" />
</ItemGroup>
```

# Example usage

Implement `GennyModule`

```C#
using Genny;
using System;

namespace Project.Templates.Default
{
    [GennyAlias("console-project")]
    [GennyModuleDescriptor("An example module")]
    public class ConsoleProjectModule : GennyModule
    {
        [GennyParameter(0, Required = true)]
        public String ClassName { get; set; }

        [GennyParameter(1, Required = true)]
        public String MethodName { get; set; }

        [GennyParameter("namespace", "n")]
        public String Namespace { get; set; }

        public ConsoleProjectModule(IServiceProvider provider)
            : base(provider)
        {
        }

        public override void Run()
        {
            GennyScaffoldingResult result = Scaffolder.Scaffold("Modules/ConsoleProject/Classes/Entry", this);

            TryWrite($"TestConsole/{ClassName}.cs", result);
        }

        public override void ShowHelp()
        {
            Logger.WriteLine("Parameters:");
            Logger.WriteLine("    1 - Scaffolded class name.");
            Logger.WriteLine("    2 - Scaffolded method name.");

            Logger.WriteLine("Named parameters:");
            Logger.WriteLine("    -n|--namespace  - Scaffolded class namespace.");
        }
    }
}
```

Which scaffolds templates from module's folder
```
Project
├── README.md                       <----- Unrelated project files
├── ...
│
└───Modules
    ├─── ConsoleProject              <----- Module root
    │    ├── Classes
    │    │   └── Entry.cshtml        <----- Module templates
    |    └── ConsoleProjectModule.cs <----- Module entry point
    │
    ├─── _ViewImports.cshtml         <----- Optional imports for module templates
    |
    ├─── Advanced                    <----- Other module roots
    │    ├── AdvancedClass.cs.cshtml
    │    ├── ...
```

Run your module from command line, by navigating to the project directory and running

```
dotnet build (only first time)
dotnet gen console-project Program Main -n TestProject
```

Which results in project structure
```
Project
├── README.md
├── ...
├── ...
├── ...
|
├── TestConsole
│   └── Program.cs
|
├── ...
|
└───Modules
    ├─── ...
    ├─── ...
    |
```