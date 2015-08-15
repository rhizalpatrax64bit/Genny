using System;
using System.IO;
using System.Threading.Tasks;

namespace Dnx.Genny.Templating
{
    public abstract class GennyTemplate<TModel>
    {
        public TModel Model { get; set; }
        protected TextWriter Output { get; set; }

        public String Execute()
        {
            using (StringWriter writer = new StringWriter())
            {
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
        public virtual void Write(Object value)
        {
            Output.Write(value ?? "");
        }
    }
}