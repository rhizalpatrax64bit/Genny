using System;

namespace Dnx.Genny
{
    public interface IGennyScaffolder
    {
        ScaffoldingResult Scaffold(String template, String project, String outputPath);
        ScaffoldingResult Scaffold(String template, String project, String outputPath, Object model);
    }
}
