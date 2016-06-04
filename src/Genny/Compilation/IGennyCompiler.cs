using System;

namespace Genny
{
    public interface IGennyCompiler
    {
        GennyCompilationResult Compile(String code);
    }
}