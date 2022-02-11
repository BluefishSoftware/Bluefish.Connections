namespace Bluefish.Connections.Interfaces;

/// <summary>
/// The IConnection interface defines the required properties and method for a Connection.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Gets the name of connection.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of connection.
    /// </summary>
    public ConnectionTypes Type { get; }
}
