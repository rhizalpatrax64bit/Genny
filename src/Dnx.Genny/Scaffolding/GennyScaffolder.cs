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
            return Scaffold(template, project, null, outputPath);
        }
        public ScaffoldingResult Scaffold(String template, String project, String outputPath, Object model)
        {
            RazorTemplateEngine engine = new RazorTemplateEngine(new GennyRazorHost());
            using (StringReader input = Read(template))
            {
                GeneratorResults results = engine.GenerateCode(input);
                if (!results.Success) return new ScaffoldingResult();

                CompilationResult result = Compiler.Compile(results.GeneratedCode);
                if (result.Errors.Any()) return new ScaffoldingResult(result.Errors);

                GennyTemplate<dynamic> gennyTemplate = Activator.CreateInstance(result.CompiledType) as GennyTemplate<dynamic>;

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