using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;

namespace Genny
{
    public class GennyCompiler : IGennyCompiler
    {
        private DependencyContext Context { get; }
        private GennyApplication Application { get; }

        public GennyCompiler(GennyApplication application)
        {
            Application = application;
            Context = DependencyContext.Load(application.Assembly);
        }

        public GennyCompilationResult Compile(String code)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                Path.GetRandomFileName(), new[] { CSharpSyntaxTree.ParseText(code) },
                GetReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream peStream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(peStream, null);
                peStream.Seek(0, SeekOrigin.Begin);

                if (!result.Success)
                {
                    IEnumerable<String> errors = result
                        .Diagnostics
                        .Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error)
                        .Select(diagnostic =>
                            diagnostic.ToString());

                    return new GennyCompilationResult(errors);
                }

                return new GennyCompilationResult(AssemblyLoadContext.Default.LoadFromStream(peStream).ExportedTypes.First());
            }
        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            String assemblyFolder = Path.GetDirectoryName(Application.Assembly.Location);
            List<MetadataReference> references = new List<MetadataReference>();

            foreach (CompilationLibrary library in Context.CompileLibraries)
            {
                try
                {
                    if (library.Type == "project")
                        foreach (String assembly in library.Assemblies)
                            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyFolder, assembly)));

                    references.AddRange(library.ResolveReferencePaths().Select(GetReference));
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }

            return references;
        }
        private MetadataReference GetReference(String path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                ModuleMetadata metadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);

                return AssemblyMetadata.Create(metadata).GetReference();
            }
        }
    }
}
