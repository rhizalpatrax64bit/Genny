using System;

namespace Genny
{
    public interface IGennyScaffolder
    {
        String ModuleRootPath { get; set; }

        GennyScaffoldingResult Scaffold(String path);
        GennyScaffoldingResult Scaffold(String path, Object model);
    }
}
