using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyScaffoldingResult
    {
        public String Content { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public GennyScaffoldingResult(String content)
        {
            Content = content;
            Errors = new String[0];
        }
        public GennyScaffoldingResult(IEnumerable<String> erorrs)
        {
            Errors = erorrs ?? new String[0];
        }
    }
}
