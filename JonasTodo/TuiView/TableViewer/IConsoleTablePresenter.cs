namespace JonasTodoConsole.TuiView.TableViewer
{
    public interface IConsoleTablePresenter
    {
        void PresentTable<T>(IEnumerable<T> tableData);
    }
}
