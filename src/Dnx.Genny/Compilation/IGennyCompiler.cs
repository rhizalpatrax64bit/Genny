using System;

namespace Dnx.Genny
{
    public interface IGennyCompiler
    {
        GennyCompilationResult Compile(String code);
    }
}