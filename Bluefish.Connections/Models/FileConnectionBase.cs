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
    /// Attempts to create a new folder.
    /// </summary>
    /// <param name="path">Relative path for the new folder.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the create was successful, other false.</returns>
    public virtual Task CreateFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

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
    /// Attempts to list direcotry entries from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the directory to list.</param>
    /// <param name="cancellationToken">Token to allow cancellation op operation.</param>
    /// <returns>An array of DirectoryEntry instances.</returns>
    public virtual Task<DirectoryEntry[]> ListAsync(string path, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    public abstract Task<bool> PutFileAsync(string path, Stream content, IEnumerable<Metadata>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the given path is at or under the specified root path.
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <param name="rootPath">the root path.</param>
    /// <returns>true if the given path is at or below the given root path</returns>
    public static bool PathIsUnderRoot(string path, string rootPath)
    {
        var pathA = path.EnsureEndsWith(Constants.PATH_SEPARATOR);
        var pathB = rootPath.EnsureEndsWith(Constants.PATH_SEPARATOR);
        return pathA.ToLower().StartsWith(pathB.ToLower());
    }

    /// <summary>
    /// Determines whether the given path is a subfolder of the root path.
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <param name="rootPath">the root path.</param>
    /// <returns>true if the given path is a sub-folder of the root path</returns>
    public static bool PathIsSubFolder(string path, string rootPath)
    {
        if (!PathIsUnderRoot(path, rootPath))
        {
            return false;
        }
        var root = rootPath.EnsureEndsWith(Constants.PATH_SEPARATOR);
        var relativePath = path[root.Length..];
        return relativePath.Contains(Constants.PATH_SEPARATOR);
    }

}
