using System;

namespace Genny
{
    public interface IGennyScaffolder
    {
        String RootPath { get; set; }

        GennyScaffoldingResult Scaffold(String path);
        GennyScaffoldingResult Scaffold(String path, Object model);
    }
}
