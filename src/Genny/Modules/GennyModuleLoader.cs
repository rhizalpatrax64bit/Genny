using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Genny
{
    public class GennyModuleLoader : IGennyModuleLoader
    {
        private IServiceProvider ServiceProvider { get; }

        public GennyModuleLoader(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public GennyModuleLoaderResult Load(GennyModuleDescriptor descriptor, String[] args)
        {
            IGennyModule module = ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type) as IGennyModule;
            Dictionary<PropertyInfo, GennyParameterAttribute> parameters = GetParameters(descriptor.Type);
            Dictionary<PropertyInfo, GennySwitchAttribute> switches = GetSwitches(descriptor.Type);
            GennyModuleLoaderResult result = new GennyModuleLoaderResult(module);

            if (!IsCorrectRequiredOrder(parameters))
            {
                result.Errors.Add("Ordered genny parameters should follow incremental order: 0 1 2 3 ...");

                return result;
            }

            if (!IsCorrectOrder(parameters))
            {
                result.Errors.Add("Ordered genny parameters can not have an optional parameter before required one.");

                return result;
            }

            foreach (KeyValuePair<PropertyInfo, GennyParameterAttribute> pair in parameters)
            {
                PropertyInfo property = pair.Key;
                GennyParameterAttribute parameter = pair.Value;
                String parameterValue = GetValue(args, parameter);

                if (parameterValue == null)
                {
                    if (parameter.Required)
                        if (parameter.Order == null)
                            result.Errors.Add($"Required {parameter.Name} parameter is not specified.");
                        else
                            result.Errors.Add($"Required parameter at position {parameter.Order} is not specified.");
                }
                else
                {
                    property.SetValue(module, Convert.ChangeType(parameterValue, property.PropertyType));
                }
            }

            foreach (KeyValuePair<PropertyInfo, GennySwitchAttribute> pair in switches)
                pair.Key.SetValue(module, GetValue(args, pair.Value));

            return result;
        }

        private Dictionary<PropertyInfo, GennyParameterAttribute> GetParameters(Type module)
        {
            return module
                .GetProperties()
                .OrderBy(property => property.GetCustomAttribute<GennyParameterAttribute>(false).Order)
                .ToDictionary(property => property, property => property.GetCustomAttribute<GennyParameterAttribute>(false));
        }
        private Dictionary<PropertyInfo, GennySwitchAttribute> GetSwitches(Type module)
        {
            return module
                .GetProperties()
                .Where(property => property.IsDefined(typeof(GennySwitchAttribute), false))
                .ToDictionary(property => property, property => property.GetCustomAttribute<GennySwitchAttribute>(false));
        }

        private String GetValue(String[] args, GennyParameterAttribute parameter)
        {
            Int32 valueIndex = parameter.Order ?? -1;
            if (valueIndex < 0)
            {
                valueIndex = Array.IndexOf(args, "--" + parameter.Name) + 1;
                if (valueIndex <= 0)
                    valueIndex = Array.IndexOf(args, "-" + parameter.ShortName) + 1;
            }
        
            if (valueIndex < 0)
                return null;

            if (args.Length <= valueIndex || IsSwitch(args[valueIndex]))
                return null;

            return args[valueIndex];
        }
        private Boolean GetValue(String[] args, GennySwitchAttribute @switch)
        {
            return args.Contains("-" + @switch.ShortName) || args.Contains("--" + @switch.Name);
        }

        private Boolean IsCorrectRequiredOrder(Dictionary<PropertyInfo, GennyParameterAttribute> parameters)
        {
            Boolean lastParameterIsRequired = true;

            foreach (KeyValuePair<PropertyInfo, GennyParameterAttribute> parameter in parameters)
            {
                if (parameter.Value.Order == null)
                    continue;

                if (parameter.Value.Required && !lastParameterIsRequired)
                    return false;

                lastParameterIsRequired = parameter.Value.Required;
            }

            return true;
        }
        private Boolean IsCorrectOrder(Dictionary<PropertyInfo, GennyParameterAttribute> parameters)
        {
            UInt16 nextOrder = 0;

            foreach (KeyValuePair<PropertyInfo, GennyParameterAttribute> parameter in parameters)
            {
                if (parameter.Value.Order == null)
                    continue;

                if (parameter.Value.Order != nextOrder++)
                    return false;
            }

            return true;
        }
        private Boolean IsSwitch(String value)
        {
            return value.StartsWith("-");
        }
    }
}
