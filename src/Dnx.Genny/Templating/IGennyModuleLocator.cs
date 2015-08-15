using System;
using System.Collections.Generic;

namespace Dnx.Genny.Templating
{
    public interface IGennyModuleLocator
    {
        IEnumerable<GennyModuleDescription> FindAll();
        IEnumerable<GennyModuleDescription> Find(String name);
    }
}