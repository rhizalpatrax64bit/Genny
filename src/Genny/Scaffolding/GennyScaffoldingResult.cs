using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyScaffoldingResult
    {
        public String Content { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public GennyScaffoldingResult()
        {
            Errors = new String[0];
        }
        public GennyScaffoldingResult(String content) : this()
        {
            Content = content;
        }
        public GennyScaffoldingResult(IEnumerable<String> erorrs)
        {
            Errors = erorrs ?? new String[0];
        }
    }
}
