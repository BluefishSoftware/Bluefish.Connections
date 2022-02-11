namespace Bluefish.Connections.Models;

public abstract class SqlConnectionBase : ISqlConnection
{
    protected SqlConnectionBase(string name, string provider)
    {
        Name = name;
        Provider = provider;
    }

    [JsonIgnore]
    public string Name { get; }

    [JsonIgnore]
    public string Provider { get; }

    [JsonIgnore]
    public ConnectionTypes Type => ConnectionTypes.SQL;

    public abstract string GetConnectionString();

    public abstract DbProviderFactory GetFactory();
}
