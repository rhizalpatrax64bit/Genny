using System;

namespace Genny
{
    public class GennyLogger : IGennyLogger
    {
        public void Write(String value)
        {
            Console.WriteLine(value);
        }
    }
}
