using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;

namespace Dnx.Genny.Compilation
{
    public class GennyRazorHost<TModel> : RazorEngineHost
    {
        public GennyRazorHost() : base(new CSharpRazorCodeLanguage())
        {
            DefaultNamespace = "Dnx.Genny.Templates";
            DefaultBaseClass = $"Dnx.Genny.Templating.GennyTemplate<{typeof(TModel).FullName}>";
            GeneratedClassContext = new GeneratedClassContext("ExecuteAsync", "Write", "WriteLiteral", new GeneratedTagHelperContext());

            NamespaceImports.Add("Dnx.Genny");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Dynamic");
            NamespaceImports.Add("System.Threading.Tasks");
            NamespaceImports.Add("System.Collections.Generic");
        }
    }
}
