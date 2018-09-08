using System;

namespace Genny
{
    public interface IGennyLogger
    {
        void Write(String value);
        void Write(String value, ConsoleColor color);

        void WriteLine(String value);
        void WriteLine(String value, ConsoleColor color);
    }
}
