using System;

namespace Genny
{
    public interface IGennyModuleLoader
    {
        GennyModuleLoaderResult Load(GennyModuleDescriptor descriptor, String[] args);
    }
}
