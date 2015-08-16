using System;
using System.Collections.Generic;

namespace Dnx.Genny
{
    public interface IGennyModuleLocator
    {
        IEnumerable<GennyModuleDescriptor> FindAll();
        IEnumerable<GennyModuleDescriptor> Find(String name);
    }
}