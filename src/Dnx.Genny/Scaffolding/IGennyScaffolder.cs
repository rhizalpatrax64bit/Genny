using System;

namespace Dnx.Genny.Scaffolding
{
    public interface IGennyScaffolder
    {
        ScaffoldingResult Scaffold(String template);
        ScaffoldingResult Scaffold<TModel>(String template, TModel model);
    }
}
