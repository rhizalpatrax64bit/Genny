using Dnx.Genny.Scaffolding;

namespace Dnx.Genny.Templating
{
    public interface IGennyModule
    {
        IScaffolder Scaffolder { get; set; }

        void Run();
    }
}
