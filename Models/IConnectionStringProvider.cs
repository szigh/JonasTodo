namespace DAL
{
    public static partial class DIRegistrations
    {
        public interface IConnectionStringProvider
        {
            string GetConnectionString();
            void SetConnectionStringKey(string key);
        }
    }
}
