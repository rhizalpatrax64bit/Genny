using System;

namespace Genny.Modules
{
    [GennyAlias("default")]
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
            String template = ReadTemplate(ModuleRoot, "Controls", "Class.cshtml");
            GennyScaffoldingResult result = Scaffolder.Scaffold(template, this);

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
