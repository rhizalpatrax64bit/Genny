using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Genny
{
    public class GennyScaffolder : IGennyScaffolder
    {
        private IGennyCompiler Compiler { get; }

        public GennyScaffolder(IGennyCompiler compiler)
        {
            Compiler = compiler;
        }

        public GennyScaffoldingResult Scaffold(String template)
        {
            return Scaffold(template, null);
        }
        public GennyScaffoldingResult Scaffold(String template, Object model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = new StringReader(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new GennyScaffoldingResult(results.ParserErrors.Select(error => error.ToString()));

                GennyCompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new GennyScaffoldingResult(result.Errors);

                Object gennyTemplate = Activator.CreateInstance(result.CompiledType);
                MethodInfo execute = gennyTemplate.GetType().GetMethod("Execute");

                return new GennyScaffoldingResult(execute.Invoke(gennyTemplate, new[] { model }) as String);
            }
        }
    }
}