namespace Bluefish.Connections.Models;

/// <summary>
/// The EmailAttachment class holds details of a single email attachment.
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// Gets or sets the filename of the attachment.
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type of the attachment.
    /// </summary>
    public string Type { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the attachment content.
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
