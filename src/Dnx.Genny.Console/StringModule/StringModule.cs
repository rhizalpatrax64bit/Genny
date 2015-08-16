using Microsoft.Framework.Runtime;
using System;

namespace Dnx.Genny.Console.Modules
{
    public class StringModule : GennyModuleBase
    {
        [GennyParameter(0, Required = true)]
        public String ClassName { get; set; }

        [GennyParameter(1, Required = true)]
        public String MethodName { get; set; }

        [GennyParameter("namespace", "n")]
        public String NameSpace { get; set; }

        public StringModule(IApplicationEnvironment environment, IGennyScaffolder scaffolder)
            : base(environment, scaffolder)
        {
        }
    }
}
