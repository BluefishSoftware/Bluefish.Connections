namespace Bluefish.Connections.Interfaces;

/// <summary>
/// The ISqlConnection interface defines the required properties and method for a SQL Connection.
/// </summary>
public interface ISqlConnection : IConnection
{
    string GetConnectionString();
    string Provider { get; }
    DbProviderFactory GetFactory();
}
