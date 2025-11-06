namespace JonasTodoConsole.TuiView.ANSI.TableViewer.Spectre.Console
{
    internal interface IRead
    {
        bool TryGet<T>(out T data);
    }
}
