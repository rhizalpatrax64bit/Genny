using Dnx.Genny.Scaffolding;
using Dnx.Genny.Services;

namespace Dnx.Genny.Templating
{
    public interface IGennyModule
    {
        ServiceProvider ServiceProvider { get; set; }
        IScaffolder Scaffolder { get; set; }

        void Run();
    }
}
