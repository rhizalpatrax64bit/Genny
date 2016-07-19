using Microsoft.AspNetCore.Razor.CodeGenerators;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Genny
{
    public class GennyScaffolder : IGennyScaffolder
    {
        public String RootPath { get; set; }
        private IGennyCompiler Compiler { get; }

        public GennyScaffolder(IGennyCompiler compiler)
        {
            Compiler = compiler;
        }

        public GennyScaffoldingResult Scaffold(String path)
        {
            return Scaffold(path, null);
        }
        public GennyScaffoldingResult Scaffold(String path, Object model)
        {
            path = Path.GetExtension(path) != ".cshtml" ? $"{path}.cshtml" : path;

            using (Stream input = File.OpenRead(Path.Combine(RootPath, path)))
            {
                GeneratorResults generation = new GennyRazorHost(RootPath).GenerateCode(path, input);
                if (!generation.Success) return new GennyScaffoldingResult(generation.ParserErrors.Select(error => error.ToString()));

                GennyCompilationResult compilation = Compiler.Compile(generation.GeneratedCode);
                if (compilation.Errors.Any()) return new GennyScaffoldingResult(compilation.Errors);

                Object template = Activator.CreateInstance(compilation.CompiledType);
                MethodInfo execute = template.GetType().GetMethod("Execute");

                return new GennyScaffoldingResult(execute.Invoke(template, new[] { model }) as String);
            }
        }
    }
}