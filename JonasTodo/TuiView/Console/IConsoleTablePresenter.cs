namespace JonasTodoConsole.TuiView.Console
{
    public interface IConsoleTablePresenter
    {
        void PresentTable<T>(IEnumerable<T> tableData);
    }
}
