﻿using System;

namespace Genny
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GennySwitchAttribute : Attribute
    {
        public String Name { get; }
        public String ShortName { get; }
        public String Description { get; set; }

        public GennySwitchAttribute(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Genny switch name should not be null or empty.");

            Name = name;
        }
        public GennySwitchAttribute(String name, String shortName) : this(name)
        {
            if (String.IsNullOrWhiteSpace(shortName))
                throw new ArgumentException("Genny switch short name should not be null or empty.");

            ShortName = shortName;
        }
    }
}
