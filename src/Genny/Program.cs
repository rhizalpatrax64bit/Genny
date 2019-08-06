using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Genny
{
    public class Program
    {
        public static void Main(String[] args)
        {
            if (args.Contains("--no-build") || Build())
            {
                args = args.Where(arg => arg != "--no-build").ToArray();

                if (args.Contains("--no-dispatch"))
                    new GennyCommand().Execute(args.Where(arg => arg != "--no-dispatch").ToArray());
                else
                    Dispatch(args);
            }
        }

        private static String ToInline(String[] args)
        {
            Int32 pendingBackslashs = 0;
            StringBuilder builder = new StringBuilder();

            for (Int32 i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    builder.Append(" ");

                if (args[i].IndexOf(' ') < 0)
                {
                    builder.Append(args[i]);

                    continue;
                }

                builder.Append("\"");

                for (Int32 j = 0; j < args[i].Length; j++)
                {
                    switch (args[i][j])
                    {
                        case '\"':
                            if (pendingBackslashs > 0)
                                builder.Append('\\', pendingBackslashs * 2);

                            builder.Append("\\\"");
                            pendingBackslashs = 0;

                            break;
                        case '\\':
                            pendingBackslashs++;

                            break;
                        default:
                            if (pendingBackslashs > 0)
                            {
                                if (pendingBackslashs == 1)
                                    builder.Append("\\");
                                else
                                    builder.Append('\\', pendingBackslashs * 2);
                            }

                            builder.Append(args[i][j]);
                            pendingBackslashs = 0;

                            break;
                    }
                }

                if (pendingBackslashs > 0)
                    builder.Append('\\', pendingBackslashs * 2);

                builder.Append("\"");
            }

            return builder.ToString();
        }
        private static void Dispatch(String[] args)
        {
            GennyApplication app = new GennyApplication();

            String deps = $"--depsfile \"{Path.Combine(app.AssemblyDirectory, $"{app.AssemblyName}.deps.json")}\"";
            String runtime = Path.Combine(app.AssemblyDirectory, $"{app.AssemblyName}.runtimeconfig.json");
            runtime = File.Exists(runtime) ? $" --runtimeconfig \"{runtime}\"" : "";
            String tool = $"\"{Assembly.GetCallingAssembly().Location}\"";

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "dotnet",
                UseShellExecute = false,
                Arguments = $"exec {deps}{runtime} {tool} --no-build --no-dispatch " + ToInline(args),
            };

            Process.Start(info).WaitForExit();
        }
        private static Boolean Build()
        {
            using (Process process = Process.Start("dotnet", "build"))
            {
                process.WaitForExit();

                Console.WriteLine();

                return process.ExitCode == 0;
            }
        }
    }
}
