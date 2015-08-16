using System;

namespace Dnx.Genny.CommandLine
{
    [Serializable]
    internal class GennyCommandLineException : Exception
    {
        public GennyCommandLineException(String message) : base(message)
        {
        }
    }
}