using System;

namespace Genny
{
    public interface IGennyScaffolder
    {
        GennyScaffoldingResult Scaffold(String template);
        GennyScaffoldingResult Scaffold(String template, Object model);
    }
}
