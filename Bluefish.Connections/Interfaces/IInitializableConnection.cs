namespace Bluefish.Connections.Interfaces;

public interface IInitializableConnection
{
    /// <summary>
    /// INitializes the connection.
    /// </summary>
    /// <param name="configuration">Configuration instance to obtain settings from.</param>
    void Initialize(IConfiguration configuration);
}
