using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.ProjectModel;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Genny
{
    public class Program
    {
        public static void Main(String[] args)
        {
            String toolName = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            String path = Path.GetFullPath(Directory.GetCurrentDirectory());
            GennyApplication application = new GennyApplication();
            Project project = ProjectReader.GetProject(path);
            application.Name = project.Name;
            application.BasePath = path;

            if (args.Contains("--no-dispatch"))
            {
                new GennyCommand(application).Execute(args.Where(arg => arg != "--no-dispatch").ToArray());
            }
            else
            {
                ProjectDependenciesCommandFactory factory = new ProjectDependenciesCommandFactory(
                    project.GetTargetFrameworks().FirstOrDefault().FrameworkName,
                    Constants.DefaultConfiguration,
                    null,
                    null,
                    path);

                factory
                    .Create(toolName, args.Concat(new[] { "--no-dispatch" }).ToArray())
                    .ForwardStdErr()
                    .ForwardStdOut()
                    .Execute();
            }
        }
    }
}
