using System;

namespace Dnx.Genny.Compilation
{
    public interface IGennyCompiler
    {
        CompilationResult Compile(String code);
    }
}