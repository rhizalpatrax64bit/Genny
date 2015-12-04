using System;

namespace Dnx.Genny.Console
{
    public class Program
    {
        private IServiceProvider ServiceProvider { get; }

        public Program(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Main()
        {
            new Genny.Program(ServiceProvider).Main(new[] { "default", "Main", "Run", "-n", "Dnx.Genny.Console.Controls" });
        }
    }
}
