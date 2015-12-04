using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Compilation.CSharp;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dnx.Genny
{
    public class GennyCompiler : IGennyCompiler
    {
        private IAssemblyLoadContext Context { get; }
        private ILibraryExporter LibraryExporter { get; }
        private IApplicationEnvironment Environment { get; }

        public GennyCompiler(IApplicationEnvironment environment, IAssemblyLoadContextAccessor accessor, ILibraryExporter exporter)
        {
            Context = accessor.GetLoadContext(typeof(GennyCompiler).GetTypeInfo().Assembly);
            LibraryExporter = exporter;
            Environment = environment;
        }

        public GennyCompilationResult Compile(String code)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                Path.GetRandomFileName(), new[] { CSharpSyntaxTree.ParseText(code) },
                GetReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream peStream = new MemoryStream())
            using (MemoryStream pdbStream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(peStream, pdbStream);

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

                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);

                return new GennyCompilationResult(Context.LoadStream(peStream, pdbStream).ExportedTypes.First());
            }
        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            return LibraryExporter
                .GetAllExports(Environment.ApplicationName)
                .MetadataReferences
                .Select(GetReference)
                .Where(reference => reference != null);
        }
        private MetadataReference GetReference(IMetadataReference metadata)
        {
            IRoslynMetadataReference roslynRef = metadata as IRoslynMetadataReference;
            if (roslynRef != null) return roslynRef.MetadataReference;

            IMetadataFileReference fileRef = metadata as IMetadataFileReference;
            if (fileRef != null) return AssemblyMetadata.CreateFromStream(File.OpenRead(fileRef.Path)).GetReference();

            IMetadataEmbeddedReference embeddedRef = metadata as IMetadataEmbeddedReference;
            if (embeddedRef != null) return MetadataReference.CreateFromImage(embeddedRef.Contents);

            IMetadataProjectReference projectRef = metadata as IMetadataProjectReference;
            if (projectRef != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    projectRef.EmitReferenceAssembly(stream);

                    return MetadataReference.CreateFromImage(stream.ToArray());
                }
            }

            return null;
        }
    }
}
