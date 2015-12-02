using System;

namespace Dnx.Genny
{
    public interface IGennyModuleLoader
    {
        GennyModuleLoaderResult Load(GennyModuleDescriptor descriptor, String[] args);
    }
}
