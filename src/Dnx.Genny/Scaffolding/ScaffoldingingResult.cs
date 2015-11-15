using System;
using System.Collections.Generic;

namespace Dnx.Genny
{
    public class ScaffoldingResult
    {
        public String Content { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public ScaffoldingResult()
        {
            Errors = new String[0];
        }
        public ScaffoldingResult(String content) : this()
        {
            Content = content;
        }
        public ScaffoldingResult(IEnumerable<String> erorrs)
        {
            Errors = erorrs ?? new String[0];
        }
    }
}
