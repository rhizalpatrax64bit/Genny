using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Genny
{
    public class GennyModuleLocator : IGennyModuleLocator
    {
        private GennyApplication Application { get; }

        public GennyModuleLocator(GennyApplication application)
        {
            Application = application;
        }

        public IEnumerable<GennyModuleDescriptor> FindAll()
        {
            return Application
                .Assembly
                .GetTypes()
                .Where(type =>
                    typeof(IGennyModule).IsAssignableFrom(type))
                .Select(type =>
                    new GennyModuleDescriptor
                    {
                        Type = type,
                        FullName = ToKebabCase(type.FullName),
                        Description = type.GetTypeInfo().GetCustomAttribute<GennyModuleDescriptorAttribute>()?.Description,
                        Name = ToKebabCase(type.GetTypeInfo().GetCustomAttribute<GennyAliasAttribute>()?.Value ?? type.Name)
                    })
                .OrderBy(descriptor =>
                    descriptor.Name);
        }
        public IEnumerable<GennyModuleDescriptor> Find(String moduleName)
        {
            return Application
                .Assembly
                .GetTypes()
                .Where(type =>
                    IsModuleMatch(type, ToKebabCase(moduleName)))
                .Select(type =>
                    new GennyModuleDescriptor
                    {
                        Type = type,
                        FullName = ToKebabCase(type.FullName),
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

            if (ToKebabCase(type.Name).Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;

            String alias = type.GetTypeInfo().GetCustomAttribute<GennyAliasAttribute>(false)?.Value;
            if (alias != null && ToKebabCase(alias).Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (ToKebabCase(type.FullName).EndsWith(name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (alias != null && ToKebabCase(type.Namespace + "." + alias).EndsWith(name, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private String ToKebabCase(String value)
        {
            return String.Join("-", Regex.Split(value, @"(?<!^)(?<=[A-Za-z0-9])(?=[A-Z])")).ToLower();
        }
    }
}
