using System;

namespace Dnx.Genny.CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GennySwitchAttribute : Attribute
    {
        public String Name { get; }
        public String ShortName { get; }

        public GennySwitchAttribute(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Genny switch name should not be null or empty.");

            Name = name;
        }
        public GennySwitchAttribute(String name, String shortName) : this(name)
        {
            ShortName = shortName;
        }
    }
}
