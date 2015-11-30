using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dnx.Genny
{
    public class GennyCommandLineParser
    {
        public void ParseTo(IGennyModule module, String[] args)
        {
            IEnumerable<PropertyInfo> parameters = module
                .GetType()
                .GetProperties()
                .Where(property => property.IsDefined(typeof(GennyParameterAttribute), false));

            IEnumerable<PropertyInfo> orderedParameters = parameters
                .Where(property => property.GetCustomAttribute<GennyParameterAttribute>(false).Order != null)
                .OrderBy(property => property.GetCustomAttribute<GennyParameterAttribute>(false).Order);

            IEnumerable<PropertyInfo> namedParameters = parameters
                .Where(property => property.GetCustomAttribute<GennyParameterAttribute>(false).Name != null);

            if (!HasCorrectOrder(orderedParameters))
                throw new GennyCommandLineException("Ordered genny parameters should follow incremental order: 0 1 2 3 ...");

            if (!HasCorrectRequiredOrder(orderedParameters))
                throw new GennyCommandLineException("Ordered genny parameters can not have an optional parameter before required one.");

            Int32 consumedParameters = 0;
            foreach (PropertyInfo property in orderedParameters)
            {
                GennyParameterAttribute parameter = property.GetCustomAttribute<GennyParameterAttribute>(false);

                if (args.Length <= parameter.Order || IsSwitch(args[parameter.Order.Value]))
                {
                    if (parameter.Required)
                        throw new GennyCommandLineException($"Could not find a required parameter at position: {parameter.Order}.");
                }
                else
                {
                    property.SetValue(module, Convert.ChangeType(args[parameter.Order.Value], property.PropertyType));

                    consumedParameters++;
                } 
            }

            args = args.Skip(consumedParameters).ToArray();

            foreach (PropertyInfo property in namedParameters)
            {
                GennyParameterAttribute parameter = property.GetCustomAttribute<GennyParameterAttribute>(false);
                String parameterValue = GetParameterValue(parameter, args);

                if (parameterValue == null)
                {
                     if (parameter.Required)
                        throw new GennyCommandLineException($"Required {parameter.Name} parameter is not specified.");
                }
                else
                {
                    property.SetValue(module, Convert.ChangeType(parameterValue, property.PropertyType));
                }
            }

            IEnumerable<PropertyInfo> switches = module
                .GetType()
                .GetProperties()
                .Where(property => property.IsDefined(typeof(GennySwitchAttribute), false));

            foreach (PropertyInfo property in switches)
            {
                GennySwitchAttribute switchAttribute = property.GetCustomAttribute<GennySwitchAttribute>(false);

                property.SetValue(module, GetSwitchValue(switchAttribute, args));
            }
        }

        private String GetParameterValue(GennyParameterAttribute parameter, IList<String> args)
        {
            Int32 parameterIndex = args.IndexOf("-" + parameter.ShortName);
            if (parameterIndex < 0)
                parameterIndex = args.IndexOf("--" + parameter.Name);
            if (parameterIndex < 0)
                return null;

            if (parameterIndex + 1 >= args.Count || IsSwitch(args[parameterIndex + 1]))
                return null;

            return args[parameterIndex + 1];
        }
        private Boolean GetSwitchValue(GennySwitchAttribute parameter, IList<String> args)
        {
            Int32 parameterIndex = args.IndexOf("-" + parameter.ShortName);
            if (parameterIndex < 0)
                parameterIndex = args.IndexOf("--" + parameter.Name);
            if (parameterIndex < 0)
                return false;

            return true;
        }

        private Boolean HasCorrectRequiredOrder(IEnumerable<PropertyInfo> orderedProperties)
        {
            Boolean lastParameterIsRequired = true;

            foreach (PropertyInfo property in orderedProperties)
            {
                GennyParameterAttribute parameter = property.GetCustomAttribute<GennyParameterAttribute>(false);
                if (parameter.Required && !lastParameterIsRequired)
                    return false;

                lastParameterIsRequired = parameter.Required;
            }

            return true;
        }
        private Boolean HasCorrectOrder(IEnumerable<PropertyInfo> orderedProperties)
        {
            UInt16 nextOrder = 0;

            foreach (PropertyInfo property in orderedProperties)
            {
                GennyParameterAttribute parameter = property.GetCustomAttribute<GennyParameterAttribute>(false);
                if (parameter.Order != nextOrder)
                    return false;

                nextOrder++;
            }

            return true;
        }

        private Boolean IsSwitch(String value)
        {
            return value.StartsWith("-");
        }
    }
}
