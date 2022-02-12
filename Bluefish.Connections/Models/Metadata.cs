namespace Bluefish.Connections.Models;

/// <summary>
/// The Metadata class holds a simple named value.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Gets or sets the name of the value.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
