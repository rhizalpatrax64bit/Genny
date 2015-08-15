using Dnx.Genny.Scaffolding;
using Dnx.Genny.Templating;
using Microsoft.Framework.Runtime;

namespace Dnx.Genny.Console.Modules
{
    public class StringModule : GennyModuleBase
    {
        public StringModule(IApplicationEnvironment environment, IGennyScaffolder scaffolder)
            : base(environment, scaffolder)
        {
        }
    }
}
