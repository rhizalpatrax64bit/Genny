using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;
using System;
using System.Diagnostics;
using System.Linq;

namespace Genny
{
    public class Program
    {
        public static void Main(String[] args)
        {
            if (args.Contains("--no-build") || Build())
                new GennyCommand().Execute(args.Where(arg => arg != "--no-build").ToArray());
        }

        private static Boolean Build()
        {
            using (Process process = Process.Start("dotnet", "build"))
            {
                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }
    }
}
