using Microsoft.AspNet.Mvc.Razor;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using Microsoft.AspNet.Razor.Parser;

namespace Dnx.Genny
{
    public class GennyRazorHost : RazorEngineHost
    {
        public GennyRazorHost() : base(new CSharpRazorCodeLanguage())
        {
            DefaultNamespace = "Dnx.Genny.Templates";
            DefaultBaseClass = "Dnx.Genny.GennyTemplate<dynamic>";
            GeneratedClassContext = new GeneratedClassContext("ExecuteAsync", "Write", "WriteLiteral", new GeneratedTagHelperContext());

            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Threading.Tasks");
            NamespaceImports.Add("System.Collections.Generic");
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            return new MvcRazorCodeParser("Dnx.Genny.GennyTemplate");
        }
    }
}
