using System;
using System.IO;
using System.Threading.Tasks;

namespace Dnx.Genny
{
    public abstract class GennyTemplate<TModel>
    {
        private String Suffix { get; set; }
        protected TModel Model { get; set; }
        protected TextWriter Output { get; set; }

        public String Execute(TModel model)
        {
            using (StringWriter writer = new StringWriter())
            {
                Model = model;
                Output = writer;
                ExecuteAsync().Wait();

                return writer.GetStringBuilder().ToString();
            }
        }
        public abstract Task ExecuteAsync();

        public void WriteLiteral(Object value)
        {
            Output.Write(value ?? "");
        }
        public void Write(Object value)
        {
            Output.Write(value ?? "");
        }

        public void BeginWriteAttribute(String name, String prefix, Int32 prefixOffset,
            String suffix, Int32 suffixOffset, Int32 attributeValuesCount)
        {
            Suffix = suffix;

            Output.Write(prefix);
        }
        public void WriteAttributeValue(String prefix, Int32 prefixOffset, Object value,
            Int32 valueOffset, Int32 valueLength, Boolean isLiteral)
        {
            Output.Write(value);
        }
        public void EndWriteAttribute()
        {
            Output.Write(Suffix);
        }
    }
}