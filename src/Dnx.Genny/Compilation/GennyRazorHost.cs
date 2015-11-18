using Microsoft.AspNet.Mvc.Razor;
using Microsoft.AspNet.Mvc.Razor.Directives;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using Microsoft.AspNet.Razor.Parser;
using System;

namespace Dnx.Genny
{
    public class GennyRazorHost : RazorEngineHost
    {
        public GennyRazorHost() : base(new CSharpRazorCodeLanguage())
        {
            DefaultNamespace = "Dnx.Genny.Templates";
            GeneratedClassContext = new GeneratedClassContext("ExecuteAsync", "Write", "WriteLiteral", new GeneratedTagHelperContext());

            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Threading.Tasks");
            NamespaceImports.Add("System.Collections.Generic");
        }
   
        public override CodeGenerator DecorateCodeGenerator(CodeGenerator incomingGenerator, CodeGeneratorContext context)
        {
            String modelType = ChunkHelper.GetModelTypeName(context.ChunkTreeBuilder.ChunkTree, "dynamic");
            DefaultBaseClass = $"Dnx.Genny.GennyTemplate<{modelType}>";

            return base.DecorateCodeGenerator(incomingGenerator, context);
        }
        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            return new MvcRazorCodeParser();
        }
    }
}
