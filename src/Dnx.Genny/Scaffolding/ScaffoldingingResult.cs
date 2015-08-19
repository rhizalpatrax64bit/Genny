using System;
using System.Collections.Generic;

namespace Dnx.Genny
{
    public class ScaffoldingResult
    {
        public String Path { get; set; }
        public String Content { get; set; }
        public IEnumerable<String> Errors { get; set; }

        public ScaffoldingResult()
        {
            Errors = new String[0];
        }
        public ScaffoldingResult(IEnumerable<String> erorrs)
        {
            Errors = erorrs ?? new String[0];
        }
        public ScaffoldingResult(String path, String content) : this()
        {
            Path = path;
            Content = content;
        }
    }
}
