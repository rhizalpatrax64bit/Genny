using Microsoft.DotNet.ProjectModel;
using System;
using System.IO;

namespace Genny.Console
{
    public class Program
    {
        public static void Main(String[] args)
        {
            GennyApplication application = new GennyApplication();
            application.BasePath = Path.GetFullPath(Directory.GetCurrentDirectory());
            application.Name = ProjectReader.GetProject(application.BasePath).Name;

            new GennyCommand(application).Execute(new[] { "default", "Main", "Run", "-n", "Genny.Console.Controls" });
        }
    }
}
