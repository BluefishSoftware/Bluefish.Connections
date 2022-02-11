namespace Bluefish.Connections.Interfaces;

/// <summary>
/// The IFileConnection interface defines the required properties and method for a File Connection.
/// </summary>
public interface IFileConnection : IConnection
{
    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>A Stream with file contents.</returns>
    Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    Task<bool> PutFileAsync(string path, Stream content, IEnumerable<NameValue>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the delete was successful, other false.</returns>
    Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default);
}
