using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Razor.Chunks;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.Razor.Parser;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyRazorHost : MvcRazorHost
    {
        public override IReadOnlyList<Chunk> DefaultInheritedChunks
        {
            get
            {
                return new List<Chunk>().AsReadOnly();
            }
        }

        public GennyRazorHost(String rootPath)
            : base(new DefaultChunkTreeCache(new PhysicalFileProvider(rootPath)), new TagHelperDescriptorResolver(false))
        {
            EnableInstrumentation = false;
            DefaultNamespace = "Genny.Templates";
            GeneratedClassContext = GeneratedClassContext.Default;
        }

        public override CodeGenerator DecorateCodeGenerator(CodeGenerator generator, CodeGeneratorContext context)
        {
            String type = ChunkHelper.GetModelTypeName(context.ChunkTreeBuilder.Root, "dynamic");
            DefaultBaseClass = $"Genny.GennyTemplate<{type}>";

            return base.DecorateCodeGenerator(generator, context);
        }
        public override ParserBase DecorateCodeParser(ParserBase parser)
        {
            return new MvcRazorCodeParser();
        }
    }
}
