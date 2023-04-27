using Dropbox.Api;
using Dropbox.Api.Files;

namespace Bluefish.Connections.File;

public class DropboxConnection : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the DropboxConnection class.
    /// </summary>
    public DropboxConnection()
        : base("Dropbox")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the Dropbox application key.
    /// </summary>
    public string AppKey { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the Dropbox application secret.
    /// </summary>
    public string AppSecret { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the access token to use.
    /// </summary>
    public string AccessToken { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the name of the root folder.
    /// </summary>
    public string RootFolder { get; set; } = String.Empty;

    #endregion

    #region IFileConnection

    /// <summary>
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the delete was successful, other false.</returns>
    public override async Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = $"{RootFolder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
        using var client = new DropboxClient(AccessToken);
        await client.Files.DeleteAsync(fullPath).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>A Stream with file contents.</returns>
    public override async Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = $"{RootFolder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
        using var client = new DropboxClient(AccessToken);
        var result = await client.Files.DownloadAsync(fullPath).ConfigureAwait(false);
        return await result.GetContentAsStreamAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    public override async Task<bool> PutFileAsync(string path, Stream content, IEnumerable<Models.Metadata>? metadata = null, CancellationToken cancellationToken = default)
    {
        // calc full path
        var fullPath = $"{RootFolder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
        using var client = new DropboxClient(AccessToken);
        // upload file - will create folders
        await client.Files.UploadAsync(fullPath, WriteMode.Overwrite.Instance, body: content).ConfigureAwait(false);
        return true;
    }

    #endregion
}
