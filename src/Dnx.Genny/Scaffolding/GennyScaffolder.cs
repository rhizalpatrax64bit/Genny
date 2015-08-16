using Dnx.Genny.Compilation;
using Dnx.Genny.Templating;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;

namespace Dnx.Genny.Scaffolding
{
    public class GennyScaffolder : IGennyScaffolder
    {
        private IGennyCompiler Compiler { get; }

        public GennyScaffolder(IGennyCompiler compiler)
        {
            Compiler = compiler;
        }

        public ScaffoldingResult Scaffold(String template)
        {
            return Scaffold(template, null);
        }
        public ScaffoldingResult Scaffold(String template, Object model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = new StringReader(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult();

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                GennyTemplate<dynamic> gennyTemplate = Activator.CreateInstance(result.CompiledType) as GennyTemplate<dynamic>;
                gennyTemplate.Model = model;

                return new ScaffoldingResult(gennyTemplate.Execute());
            }
        }
    }
}