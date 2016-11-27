using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Genny
{
    public class GennyCompiler : IGennyCompiler
    {
        private DependencyContext Context { get; }

        public GennyCompiler(GennyApplication application)
        {
            Context = DependencyContext.Load(Assembly.Load(new AssemblyName(application.Name)));
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

#if NET46
                return new GennyCompilationResult(Assembly.Load(peStream.ToArray()).ExportedTypes.First());
#else
                return new GennyCompilationResult(System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(peStream).ExportedTypes.First());
#endif
            }
        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            List<MetadataReference> references = new List<MetadataReference>();
            foreach (CompilationLibrary library in Context.CompileLibraries)
            {
                try
                {
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
