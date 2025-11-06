namespace JonasTodoConsole.TuiView.DataInterfaces
{
    internal interface ITopicsTable
    {
        bool TryUpdate<T>(int id, Func<T, T> updateFunction);
    }
}