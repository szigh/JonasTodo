namespace JonasTodoConsole.TuiView.ANSI.TableViewer.Spectre.Console
{
    internal interface IUpdate
    {
        bool TryUpdate<T>(int id, Func<T,T> updateFunction);
    }
}
