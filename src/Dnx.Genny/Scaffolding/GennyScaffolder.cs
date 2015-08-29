using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.CodeGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Runtime;
using System;
using System.IO;
using System.Linq;

namespace Dnx.Genny
{
    public class GennyScaffolder : IGennyScaffolder
    {
        private IGennyCompiler Compiler { get; }
        private IApplicationEnvironment Environment { get; }

        public GennyScaffolder(IApplicationEnvironment environment, IGennyCompiler compiler)
        {
            Environment = environment;
            Compiler = compiler;
        }

        public ScaffoldingResult Scaffold(String template, String project, String outputPath)
        {
            return Scaffold<Object>(template, project, outputPath, null);
        }
        public ScaffoldingResult Scaffold<T>(String template, String project, String outputPath, T model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = Read(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult(results.ParserErrors.Select(error => error.ToString()));

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                GennyTemplate<T> gennyTemplate = Activator.CreateInstance(result.CompiledType) as GennyTemplate<T>;

                return new ScaffoldingResult(FormResultPath(project, outputPath), gennyTemplate.Execute(model));
            }
        }

        private String FormResultPath(String project, String outputPath)
        {
            String path = Path.GetDirectoryName(Environment.ApplicationBasePath);
            path = Path.Combine(path, project, outputPath);

            return path;
        }
        private StringReader Read(String template)
        {
            String path = Path.Combine(Environment.ApplicationBasePath, template);
            StringReader reader = new StringReader(File.ReadAllText(path));

            return reader;
        }
    }
}