using System;

namespace Genny
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GennyAliasAttribute : Attribute
    {
        public String Value { get; }

        public GennyAliasAttribute(String value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Genny alias should not be null or empty.");

            Value = value;
        }
    }
}
