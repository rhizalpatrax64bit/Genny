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
    public class Scaffolder : IScaffolder
    {
        private IGennyCompiler Compiler { get; }

        public Scaffolder(IGennyCompiler compiler)
        {
            Compiler = compiler;
        }

        public ScaffoldingResult Scaffold<TModel>(String template, TModel model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost<TModel>());
            using (StringReader input = new StringReader(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult();

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                GennyTemplate<TModel> gennyTemplate = Activator.CreateInstance(result.CompiledType) as GennyTemplate<TModel>;
                gennyTemplate.Model = model;

                return new ScaffoldingResult(gennyTemplate.Execute());
            }
        }
    }
}