Code generator for DNX projects.

This project is currently in development.

# Installation

Locate the commands section in `project.json` and add the gen command:

```JSON
"commands": {
  "gen": "Dnx.Genny"
}
```

# Default usage

Implement `GennyModuleBase`

```C#
using Dnx.Genny;
using Microsoft.Framework.Runtime;

namespace Project.GennyModules.Default
{
    [GennyParameter(0, Required = true)]
    public String ClassName { get; set; }

    [GennyParameter(1, Required = true)]
    public String MethodName { get; set; }

    [GennyParameter("namespace", "n")]
    public String NameSpace { get; set; }

    public class DefaultModule : GennyModuleBase
    {
        public DefaultModule(IServiceProvider provider)
            : base(provider)
        {
        }
    }
}
```

Which scaffolds templates from module's folder
```
Project
├── README.md                           <----- Unrelated project files
├── ...
├── ...
├── ...   
│
└───GennyModules
    ├───Default                         <----- Default module folder
    │   ├── MainFolder                  <----- Default module template's folder
    │   │   └── DefaultClass.cs.cshtml  <----- Default module template
    │   │
    │   ├── DefaultHelper.cs.cshtml     <----- Default module template
    |   └── DefaultModule.cs            <----- Default module class
    │   
    ├───Advanced                        <----- Other unrelated modules
    │   ├── AdvancedClass.cs.cshtml
    │   │   ...
```

Run your module from command line, by navigating to the project directory and running

```
dnx gen default ClassName MethodName -n Namespace.Default
```

Which results in project structure
```
Project
├── README.md
├── ...
├── ...
├── ...
|
├── MainFolder
│   └── DefaultClass.cs
├── DefaultHelper.cs
|
└───GennyModules
    ├─── ...
    ├─── ...
    |
```

# Advanced usage

Implement `IGennyModule`

```C#
using System;
using Dnx.Genny;
using System.Linq;

namespace Project.GennyModules.Advanced
{
    public class MyGennyModule : IGennyModule
    {
        public IGennyScaffolder Scaffolder { get; set; }

        public MyGennyModule(IGennyScaffolder scaffolder)
        {
            Scaffolder = scaffolder;
        }

        public void Run()
        {
            ScaffoldingResult result = Scaffolder.Scaffold("C:\\MyProject\\MainFolder\\DefaultClass.cs.cshtml", "MyProject", "Main\\Default.cs", "Model");

            if (result.Errors.Any())
            {
                // Failed to scaffold, write out error or do nothing
            }
            else
            {
                // Write(result); // Writes result.Content to result.Path
                // Or write results to any other source
            }
        }
    }
}
```

Run your module from command line, by navigating to the project directory and running

```
dnx gen my-genny-module
```