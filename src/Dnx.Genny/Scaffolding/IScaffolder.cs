using System;

namespace Dnx.Genny.Scaffolding
{
    public interface IScaffolder
    {
        ScaffoldingResult Scaffold<TModel>(String template, TModel model);
    }
}
