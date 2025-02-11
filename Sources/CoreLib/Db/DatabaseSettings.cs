namespace CoreLib.Db;

public class DatabaseSettings
{
    public string MasterConnection { get; set; }
    public string[] ReplicaConnections { get; set; }
}