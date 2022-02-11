namespace Bluefish.Connections.Models;

public abstract class FileConnectionBase : IFileConnection
{
    protected FileConnectionBase(string name)
    {
        Name = name;
    }

    [JsonIgnore]
    public string Name { get; private set; }

    [JsonIgnore]
    public ConnectionTypes Type => ConnectionTypes.File;

    /// <summary>
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    public abstract Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns></returns>
    public abstract Task<Stream> GetFileAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    public abstract Task<bool> PutFileAsync(string path, Stream content, IEnumerable<NameValue>? metadata = null, CancellationToken cancellationToken = default);
}
