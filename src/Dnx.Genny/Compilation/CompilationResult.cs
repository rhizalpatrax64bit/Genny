using System;
using System.Collections.Generic;

namespace Dnx.Genny
{
    public class CompilationResult
    {
        public String Code { get; set; }
        public Type CompiledType { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public CompilationResult()
        {
            Errors = new String[0];
        }
        public CompilationResult(Type type) : this()
        {
            CompiledType = type;
        }
        public CompilationResult(IEnumerable<String> errors)
        {
            Errors = errors ?? new String[0];
        }
    }
}
