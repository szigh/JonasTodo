namespace JonasTodoConsole.TuiView.ANSI.DataInterfaces
{
    internal interface ITopicsTable
    {
        bool TryUpdate<T>(int id, Func<T, T> updateFunction);
    }
}