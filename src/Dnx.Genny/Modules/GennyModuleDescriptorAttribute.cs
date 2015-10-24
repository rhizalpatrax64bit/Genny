using System;

namespace Dnx.Genny
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
