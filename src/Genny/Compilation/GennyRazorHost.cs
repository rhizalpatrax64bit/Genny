using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.Razor.Parser;
using System;

namespace Genny
{
    public class GennyRazorHost : RazorEngineHost
    {
        public GennyRazorHost() : base(new CSharpRazorCodeLanguage())
        {
            DefaultNamespace = "Genny.Templates";
            GeneratedClassContext = new GeneratedClassContext("ExecuteAsync", "Write", "WriteLiteral", new GeneratedTagHelperContext());

            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Threading.Tasks");
            NamespaceImports.Add("System.Collections.Generic");
        }
   
        public override CodeGenerator DecorateCodeGenerator(CodeGenerator incomingGenerator, CodeGeneratorContext context)
        {
            String modelType = ChunkHelper.GetModelTypeName(context.ChunkTreeBuilder.Root, "dynamic");
            DefaultBaseClass = $"Genny.GennyTemplate<{modelType}>";

            return base.DecorateCodeGenerator(incomingGenerator, context);
        }
        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            return new MvcRazorCodeParser();
        }
    }
}
