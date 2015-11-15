using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using System;
using System.IO;
using System.Linq;

namespace Dnx.Genny
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
            return Scaffold<Object>(template, null);
        }
        public ScaffoldingResult Scaffold<T>(String template, T model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = new StringReader(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult(results.ParserErrors.Select(error => error.ToString()));

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                GennyTemplate<T> gennyTemplate = Activator.CreateInstance(result.CompiledType) as GennyTemplate<T>;

                return new ScaffoldingResult(gennyTemplate.Execute(model));
            }
        }
    }
}