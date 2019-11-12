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
        public String Framework { get; set; }
        public Assembly Assembly { get; set; }
        public String AssemblyName { get; set; }
        public String AssemblyDirectory { get; set; }

        public GennyApplication()
        {
            String projectFile = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.csproj").First();
            XDocument project = XDocument.Load(projectFile);
            BasePath = Directory.GetCurrentDirectory();

            Framework = project.Descendants("TargetFrameworks").FirstOrDefault()?.Value.Split(':').First();
            Framework = Framework ?? project.Descendants("TargetFramework").First().Value;

            AssemblyName = project.Descendants("AssemblyName").FirstOrDefault()?.Value;
            AssemblyName = AssemblyName ?? Path.GetFileNameWithoutExtension(projectFile);

            AssemblyDirectory = $"{BasePath}{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}{Framework}";

            Assembly = Assembly.LoadFrom($"{AssemblyDirectory}{Path.DirectorySeparatorChar}{AssemblyName}.dll");
        }
    }
}
