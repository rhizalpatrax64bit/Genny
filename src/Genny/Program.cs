using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;
using System;

namespace Genny
{
    public class Program
    {
        public static void Main(String[] args)
        {
            new GennyCommand().Execute(args);
        }
    }
}
