namespace DataFlow.Common.Connections;

public class WindowsShareConnection : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the WindowsShareConnection class.
    /// </summary>
    public WindowsShareConnection()
        : base("Windows File Share")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the name of the Server the share is located on.
    /// </summary>
    [Display(Name = "Server Name")]
    [Required]
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the share.
    /// </summary>
    [Display(Name = "Share Name")]
    [Required]
    public string ShareName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the root folder in the share.
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
        File.Delete(path);
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
        var shareName = ShareName.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');
        var rootPath = RootPath.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');
        var subPath = path.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');

        // this provider expects paths relative to the network share folder root
        var fullPath = $"\\\\{ServerName}\\{shareName}\\{rootPath}\\{subPath}";

        // async read all bytes and return
        return Task.FromResult((Stream)File.Open(fullPath, FileMode.Open, FileAccess.Read));
    }

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    public override async Task<bool> PutFileAsync(string path, Stream content, IEnumerable<NameValue>? metadata = null, CancellationToken cancellationToken = default)
    {
        // calculate path
        var shareName = ShareName.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');
        var rootPath = RootPath.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');
        var subPath = path.Replace("/", "\\").TrimStart('\\').TrimEnd('\\');

        // path is relative to root
        var fullPath = $"\\\\{ServerName}\\{shareName}\\{rootPath}\\{subPath}";

        // ensure folder exists
        var dirPath = Path.GetDirectoryName(fullPath);
        if (dirPath != null && !Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // async write all bytes and return path
        using FileStream targetStream = File.Open(fullPath, FileMode.Create);
        await content.CopyToAsync(targetStream).ConfigureAwait(false);
        return true;
    }

    #endregion
}
