using System;

namespace Dnx.Genny
{
    internal class GennyCommandLineException : Exception
    {
        public GennyCommandLineException(String message) : base(message)
        {
        }
    }
}