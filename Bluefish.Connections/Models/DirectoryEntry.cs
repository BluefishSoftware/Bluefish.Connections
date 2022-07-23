namespace Bluefish.Connections.Models;

public class DirectoryEntry
{
    /// <summary>
    /// Gets or sets an alias for this entry.
    /// </summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// Gets the directory name.
    /// </summary>
    public string DirectoryName => System.IO.Path.GetDirectoryName(Path) ?? $"{Constants.PATH_SEPARATOR}";

    /// <summary>
    /// Gets the directory name.
    /// </summary>
    public string Extension => System.IO.Path.GetExtension(Path);

    /// <summary>
    /// Gets the filename.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Gets the filename without extension.
    /// </summary>
    public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Path);

    /// <summary>
    /// Gets or sets a UTC date and tgime of when the file was last modified.
    /// </summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the fullpath of the directory entry.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size (in bytes) of the directory entry.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the type of diretcory entry.
    /// </summary>
    public DirectoryEntryTypes Type { get; set; }
}
