using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyCompilationResult
    {
        public String Code { get; set; }
        public Type CompiledType { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public GennyCompilationResult()
        {
            Errors = new String[0];
        }
        public GennyCompilationResult(Type type) : this()
        {
            CompiledType = type;
        }
        public GennyCompilationResult(IEnumerable<String> errors)
        {
            Errors = errors ?? new String[0];
        }
    }
}
