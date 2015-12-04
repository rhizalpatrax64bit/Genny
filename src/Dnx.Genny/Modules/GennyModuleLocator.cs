using Microsoft.Extensions.PlatformAbstractions;
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
                        Description = type.GetTypeInfo().GetCustomAttribute<GennyModuleDescriptorAttribute>()?.Description,
                        Name = ToKebabCase(type.GetTypeInfo().GetCustomAttribute<GennyAliasAttribute>()?.Value ?? type.Name)
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
                    IsModuleMatch(type, moduleName))
                .Select(type =>
                    new GennyModuleDescriptor
                    {
                        Type = type,
                        Description = type.GetTypeInfo().GetCustomAttribute<GennyModuleDescriptorAttribute>()?.Description,
                        Name = ToKebabCase(type.GetTypeInfo().GetCustomAttribute<GennyAliasAttribute>()?.Value ?? type.Name)
                    })
                .OrderBy(descriptor =>
                    descriptor.Name);
        }

        private Boolean IsModuleMatch(Type type, String name)
        {
            if (!typeof(IGennyModule).IsAssignableFrom(type))
                return false;

            if (String.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (String.Equals(type.FullName, name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (String.Equals(ToKebabCase(type.Name), name, StringComparison.OrdinalIgnoreCase))
                return true;

            String alias = type.GetTypeInfo().GetCustomAttribute<GennyAliasAttribute>(false)?.Value;
            if (alias != null && String.Equals(ToKebabCase(alias), name, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private String ToKebabCase(String typeName)
        {
            return String.Join("-", Regex.Split(typeName, @"(?<!^)(?=[A-Z])").Select(name => name)).ToLower();
        }
    }
}