namespace Bluefish.Connections.Models;

/// <summary>
/// An enumeration of possible connection types.
/// </summary>
public enum ConnectionTypes
{
    /// <summary>
    /// The connection is to an Email provider.
    /// </summary>
    Email,
    /// <summary>
    /// The connection is a connection to a SQL database supported by a .Net data provider.
    /// </summary>
    SQL,
    /// <summary>
    /// The connection is to a file storage provider.
    /// </summary>
    File
}
