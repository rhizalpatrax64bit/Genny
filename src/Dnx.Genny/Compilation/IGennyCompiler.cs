using System;

namespace Dnx.Genny
{
    public interface IGennyCompiler
    {
        CompilationResult Compile(String code);
    }
}