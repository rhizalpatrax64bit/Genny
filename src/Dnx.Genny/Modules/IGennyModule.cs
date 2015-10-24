namespace Dnx.Genny
{
    public interface IGennyModule
    {
        void Run();

        void ShowHelp(IGennyLogger logger);
    }
}
