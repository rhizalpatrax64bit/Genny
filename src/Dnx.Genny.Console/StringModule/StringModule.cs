using System;

namespace Dnx.Genny.Modules
{
    [GennyModuleDescriptor("An example module")]
    public class StringModule : GennyModuleBase
    {
        [GennyParameter(0, Required = true)]
        public String ClassName { get; set; }

        [GennyParameter(1, Required = true)]
        public String MethodName { get; set; }

        [GennyParameter("namespace", "n")]
        public String Namespace { get; set; }

        public StringModule(IServiceProvider provider)
            : base(provider)
        {
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
