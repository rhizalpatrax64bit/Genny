using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyModuleLoaderResult
    {
        public IGennyModule Module { get; }
        public IList<String> Errors { get; set; }

        public GennyModuleLoaderResult(IGennyModule module)
        {
            Module = module;
            Errors = new List<String>();
        }
    }
}