using System;

namespace Genny.Modules
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
