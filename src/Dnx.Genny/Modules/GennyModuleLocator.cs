using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dnx.Genny
{
    public class GennyModuleLocator : IGennyModuleLocator
    {
        private String ApplicationName { get; }

        public GennyModuleLocator(IApplicationEnvironment environment)
        {
            ApplicationName = environment.ApplicationName;
        }

        public IEnumerable<GennyModuleDescriptor> FindAll()
        {
            return Assembly
                .Load(new AssemblyName(ApplicationName))
                .GetTypes()
                .Where(type =>
                    typeof(IGennyModule).IsAssignableFrom(type))
                .Select(type =>
                    new GennyModuleDescriptor
                    {
                        Type = type,
                        Name = ToKebabCase(type.Name)
                    })
                .OrderBy(descriptor =>
                    descriptor.Name);
        }
        public IEnumerable<GennyModuleDescriptor> Find(String moduleName)
        {
            return Assembly
                .Load(new AssemblyName(ApplicationName))
                .GetTypes()
                .Where(type =>
                    typeof(IGennyModule).IsAssignableFrom(type) &&
                    (String.Equals(type.Name, moduleName, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(ToKebabCase(type.Name), moduleName, StringComparison.OrdinalIgnoreCase)))
                .Select(type =>
                    new GennyModuleDescriptor
                    {
                        Type = type,
                        Name = ToKebabCase(type.Name)
                    })
                .OrderBy(descriptor =>
                    descriptor.Name);
        }

        private String ToKebabCase(String typeName)
        {
            return String.Join("-", Regex.Split(typeName, @"(?<!^)(?=[A-Z])").Select(name => name.ToLower()));
        }
    }
}