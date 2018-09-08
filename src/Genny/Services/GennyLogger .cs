using System;

namespace Genny
{
    public class GennyLogger : IGennyLogger
    {
        public void Write(String value)
        {
            Console.Write(value);
        }
        public void Write(String value, ConsoleColor color)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(value);

            Console.ForegroundColor = original;
        }

        public void WriteLine(String value)
        {
            Console.WriteLine(value);
        }
        public void WriteLine(String value, ConsoleColor color)
        {
            Write(value, color);

            Console.WriteLine();
        }
    }
}
