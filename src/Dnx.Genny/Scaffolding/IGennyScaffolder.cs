using System;

namespace Dnx.Genny.Scaffolding
{
    public interface IGennyScaffolder
    {
        ScaffoldingResult Scaffold(String template);
        ScaffoldingResult Scaffold(String template, Object model);
    }
}
