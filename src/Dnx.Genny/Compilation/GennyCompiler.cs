using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Compilation;
using Microsoft.Framework.Runtime.Roslyn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dnx.Genny.Compilation
{
    public class GennyCompiler : IGennyCompiler
    {
        private IAssemblyLoadContext Loader { get; }
        private ILibraryManager LibraryManager { get; }
        private IApplicationEnvironment Environment { get; }

        public GennyCompiler(IApplicationEnvironment environment, IAssemblyLoadContextAccessor accessor, ILibraryManager manager)
        {
            Loader = accessor.GetLoadContext(typeof(GennyCompiler).GetTypeInfo().Assembly);
            Environment = environment;
            LibraryManager = manager;
        }

        public CompilationResult Compile(String code)
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

                    return new CompilationResult(errors);
                }

                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);

                return new CompilationResult(Loader.LoadStream(peStream, pdbStream).ExportedTypes.First());
            }
        }

        private List<MetadataReference> GetReferences()
        {
            String[] projects = new String[] { Environment.ApplicationName, "Dnx.Genny" };

            return projects
                .SelectMany(project => LibraryManager.GetAllExports(project).MetadataReferences)
                .Select(reference => GetReference(reference))
                .ToList();
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

            throw new NotSupportedException("Not supported reference.");
        }
    }
}
