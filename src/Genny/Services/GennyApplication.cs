using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Genny
{
    public class GennyApplication
    {
        public String BasePath { get; set; }
        public Assembly Assembly { get; set; }

        public GennyApplication()
        {
            String projectFile = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.csproj").First();
            XDocument project = XDocument.Load(projectFile);
            BasePath = Directory.GetCurrentDirectory();

            String framework = project.Descendants("TargetFrameworks").FirstOrDefault()?.Value.Split(':').First();
            framework = framework ?? project.Descendants("TargetFramework").First().Value;

            String assemblyName = project.Descendants("AssemblyName").FirstOrDefault()?.Value;
            assemblyName = assemblyName ?? Path.GetFileNameWithoutExtension(projectFile);

            Assembly = Assembly.LoadFrom($"{BasePath}\\bin\\Debug\\{framework}\\{assemblyName}.dll");
        }
    }
}
