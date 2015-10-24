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
        public String NameSpace { get; set; }

        public StringModule(IServiceProvider provider)
            : base(provider)
        {
        }
    }
}
