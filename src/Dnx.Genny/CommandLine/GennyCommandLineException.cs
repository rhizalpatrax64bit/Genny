using System;

namespace Dnx.Genny
{
    [Serializable]
    internal class GennyCommandLineException : Exception
    {
        public GennyCommandLineException(String message) : base(message)
        {
        }
    }
}