
namespace JonasTodoConsole.TuiView.Tables
{
    public interface ITopicsTable
    {
        Task RunAsync(CancellationToken ct = default);
    }
}