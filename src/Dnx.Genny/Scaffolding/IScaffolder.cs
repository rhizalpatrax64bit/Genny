using System;

namespace Dnx.Genny.Scaffolding
{
    public interface IScaffolder
    {
        ScaffoldingResult Scaffold(String template);
        ScaffoldingResult Scaffold<TModel>(String template, TModel model);
    }
}
