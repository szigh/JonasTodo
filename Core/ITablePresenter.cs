namespace Core
{
    internal interface ITablePresenter
    {
        void PresentTable<T>(IEnumerable<T> tableData);
    }
}
