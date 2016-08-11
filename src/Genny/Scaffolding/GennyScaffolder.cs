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
        private GennyApplication Application { get; }

        public GennyScaffolder(GennyApplication application, IGennyCompiler compiler)
        {
            Application = application;
            Compiler = compiler;
        }

        public GennyScaffoldingResult Scaffold(String path)
        {
            return Scaffold(path, null);
        }
        public GennyScaffoldingResult Scaffold(String path, Object model)
        {
            path = Path.GetExtension(path) != ".cshtml" ? $"{path}.cshtml" : path;

            using (Stream input = File.OpenRead(Path.Combine(Application.BasePath, path)))
            {
                GeneratorResults generation = new GennyRazorHost(Application.BasePath).GenerateCode(path, input);
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