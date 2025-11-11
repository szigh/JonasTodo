namespace DAL
{
    public class DALSettings
    {
        public required string LocalConnectionString { get; set; }
        public required string RemoteConnectionString { get; set; }
        public int SomeOtherSetting { get; set; }
    }
}
