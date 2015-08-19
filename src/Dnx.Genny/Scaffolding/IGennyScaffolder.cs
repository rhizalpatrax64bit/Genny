using System;

namespace Dnx.Genny
{
    public interface IGennyScaffolder
    {
        ScaffoldingResult Scaffold(String path, String template);
        ScaffoldingResult Scaffold(String path, String template, Object model);
    }
}
