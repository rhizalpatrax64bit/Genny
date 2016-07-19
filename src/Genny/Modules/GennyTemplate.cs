using System;
using System.IO;
using System.Threading.Tasks;

namespace Genny
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
    }
}