using System;

namespace Genny
{
    public interface IGennyScaffolder
    {
        GennyScaffoldingResult Scaffold(String path);
        GennyScaffoldingResult Scaffold(String path, Object model);
    }
}
