using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Language;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Genny
{
    public class GennyScaffolder : IGennyScaffolder
    {
        public String ModuleRootPath { get; set; }

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
            path = Path.Combine(Application.BasePath, Path.GetExtension(path) != ".cshtml" ? $"{path}.cshtml" : path);

            using (Stream input = File.OpenRead(path))
            {
                RazorSourceDocument source = RazorSourceDocument.Create(File.ReadAllText(path), "");
                RazorCodeDocument document = RazorCodeDocument.Create(source, GetTemplateImports());
                RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create(Application.BasePath), builder =>
                {
                    builder.SetBaseType($"GennyTemplate<{model?.GetType().FullName ?? "object"}>");
                    builder.SetNamespace("Genny.Templates");
                    ModelDirective.Register(builder);
                }).Engine.Process(document);

                RazorCSharpDocument generation = document.GetCSharpDocument();

                if (generation.Diagnostics.Any())
                    return new GennyScaffoldingResult(generation.Diagnostics.Select(diagnostic => diagnostic.GetMessage()));

                GennyCompilationResult compilation = Compiler.Compile(generation.GeneratedCode);
                if (compilation.Errors.Any())
                    return new GennyScaffoldingResult(compilation.Errors);

                Object template = Activator.CreateInstance(compilation.CompiledType);
                MethodInfo execute = template.GetType().GetMethod("Execute");

                return new GennyScaffoldingResult(execute.Invoke(template, new[] { model }) as String);
            }
        }

        private RazorSourceDocument[] GetTemplateImports()
        {
            String rootDirectory = ModuleRootPath;
            DirectoryInfo root = new DirectoryInfo(ModuleRootPath);
            while (root != null && root.Parent?.FullName != Application.BasePath)
                rootDirectory = (root = root.Parent)?.FullName;

            return Directory
                .GetFiles(rootDirectory ?? ModuleRootPath, "_ViewImports.cshtml")
                .Select(path => RazorSourceDocument.Create(File.ReadAllText(path), path))
                .Append(RazorSourceDocument.Create(
                    "@using System;\n" +
                    "@using System.Linq;\n" +
                    "@using System.Threading.Tasks;\n" +
                    "@using System.Collections.Generic;\n", "_GennyImports.cshtml"))
                .ToArray();
        }
    }
}
