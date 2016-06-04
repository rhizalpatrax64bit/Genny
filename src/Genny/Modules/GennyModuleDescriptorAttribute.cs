using System;

namespace Genny
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GennyModuleDescriptorAttribute : Attribute
    {
        public String Description { get; }

        public GennyModuleDescriptorAttribute(String description)
        {
            Description = description;
        }
    }
}
