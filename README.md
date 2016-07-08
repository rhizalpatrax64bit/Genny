Code generator for DotNet projects.

# Installation

Add genny dependencies to `project.json`:

```JSON
"dependencies": {
  "Genny": "1.0.0"
}

"tools": {
  "Genny": {
    "version": "1.0.0"
  }
}
```

# Example usage

Implement `GennyModule`

```C#
using Genny;
using System;

namespace Project.Templates.Default
{
    [GennyAlias("Default")]
    [GennyModuleDescriptor("An example module")]
    public class DefaultModule : GennyModule
    {
        [GennyParameter(0, Required = true)]
        public String ClassName { get; set; }

        [GennyParameter(1, Required = true)]
        public String MethodName { get; set; }

        [GennyParameter("namespace", "n")]
        public String Namespace { get; set; }

        public DefaultModule(IServiceProvider provider)
            : base(provider)
        {
        }

        public override void Run()
        {
            GennyScaffoldingResult result = Scaffolder.Scaffold("Main/Class", this);

            TryWrite($"Controls/{ClassName}.cs", result);
        }

        public override void ShowHelp(IGennyLogger logger)
        {
            logger.Write("Parameters:");
            logger.Write("    1 - Scaffolded class name.");
            logger.Write("    2 - Scaffolded method name.");

            logger.Write("Named parameters:");
            logger.Write("    -n|--namespace  - Scaffolded class namespace.");
        }
    }
}
```

Which scaffolds templates from module's folder
```
Project
├── README.md                       <----- Unrelated project files
├── ...
├── ...
├── ...   
│
└───GennyModules
    ├───Default                     <----- Default module folder
    │   ├── Main                    <----- Default module template's folder
    │   │   └── Class.cshtml        <----- Default module template
    |   └── DefaultModule.cs        <----- Default module class
    │   
    ├───Advanced                    <----- Other unrelated modules
    │   ├── AdvancedClass.cs.cshtml
    │   │   ...
```

Run your module from command line, by navigating to the project directory and running

```
dotnet gen default Main Run -n Project.Controls
```

Which results in project structure
```
Project
├── README.md
├── ...
├── ...
├── ...
|
├── Controls
│   └── Main.cs
|
├── ...
|
└───GennyModules
    ├─── ...
    ├─── ...
    |
```