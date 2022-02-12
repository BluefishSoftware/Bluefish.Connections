namespace Bluefish.Connections.File;

public class LocalFileSystemConnection : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the LocalFileSystemConnection class.
    /// </summary>
    public LocalFileSystemConnection()
        : base("Local File System")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the name of the root folder on the local file system.
    /// </summary>
    [Display(Name = "Root Folder")]
    [Required]
    public string RootPath { get; set; } = "/";

    #endregion

    #region IFileConnection

    /// <summary>
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the delete was successful, other false.</returns>
    public override Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        System.IO.File.Delete(path);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>A Stream with file contents.</returns>
    public override Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // calculate path
        var rootPath = RootPath.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        var subPath = path.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);

        // this provider expects paths relative to the root path
        var fullPath = $"{rootPath}{Path.DirectorySeparatorChar}{subPath}";

        // async read all bytes and return
        return Task.FromResult((Stream)System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read));
    }

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    public override async Task<bool> PutFileAsync(string path, Stream content, IEnumerable<Metadata>? metadata = null, CancellationToken cancellationToken = default)
    {
        // calculate path
        var rootPath = RootPath.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        var subPath = path.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);

        // this provider expects paths relative to the root path
        var fullPath = $"{rootPath}{Path.DirectorySeparatorChar}{subPath}";

        // ensure folder exists
        var dirPath = Path.GetDirectoryName(fullPath);
        if (dirPath != null && !Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // async write all bytes and return path
        using FileStream targetStream = System.IO.File.Open(fullPath, FileMode.Create);
        await content.CopyToAsync(targetStream).ConfigureAwait(false);
        return true;
    }

    #endregion
}
