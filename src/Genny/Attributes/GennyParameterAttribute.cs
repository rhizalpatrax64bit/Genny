﻿using System;

namespace Genny
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GennyParameterAttribute : Attribute
    {
        public String Name { get; }
        public UInt16? Order { get; }
        public String ShortName { get; }
        public Boolean Required { get; set; }
        public String Description { get; set; }
        public String DefaultValue { get; set; }

        public GennyParameterAttribute(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Genny parameter name should not be null or empty.");

            Name = name;
        }
        public GennyParameterAttribute(UInt16 order)
        {
            Order = order;
        }
        public GennyParameterAttribute(String name, String shortName) : this(name)
        {
            if (String.IsNullOrWhiteSpace(shortName))
                throw new ArgumentException("Genny parameter short name should not be null or empty.");

            ShortName = shortName;
        }
    }
}
