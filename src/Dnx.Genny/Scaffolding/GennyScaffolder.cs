using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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
            return Scaffold(template, null);
        }
        public ScaffoldingResult Scaffold(String template, Object model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = new StringReader(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult(results.ParserErrors.Select(error => error.ToString()));

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                Object gennyTemplate = Activator.CreateInstance(result.CompiledType);
                MethodInfo execute = gennyTemplate.GetType().GetMethod("Execute");

                return new ScaffoldingResult(execute.Invoke(gennyTemplate, new[] { model }) as String);
            }
        }
    }
}