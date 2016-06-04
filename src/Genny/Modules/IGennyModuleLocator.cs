using System;
using System.Collections.Generic;

namespace Genny
{
    public interface IGennyModuleLocator
    {
        IEnumerable<GennyModuleDescriptor> FindAll();
        IEnumerable<GennyModuleDescriptor> Find(String name);
    }
}