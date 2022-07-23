using Amazon.S3;
using Amazon.S3.Model;

namespace Bluefish.Connections.File;

/// <summary>
/// The DigitalOceanSpacesConnection class implements a connection that stores and retrieves files to
/// a DigitalOcean Spaces account.
/// </summary>
public class DigitalOceanSpacesConnection : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the DigitalOceanSpacesConnection class.
    /// </summary>
    public DigitalOceanSpacesConnection()
        : base("DigitalOcean Spaces")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the access key for the account.
    /// </summary>
    public string AccessKey { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the secret for the access key.
    /// </summary>
    public string Secret { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the endpoint of the service.
    /// </summary>
    public string ServiceUrl { get; set; } = "ams3.digitaloceanspaces.com";

    /// <summary>
    /// Gets or sets the name of the Space (AWS S3 Bucket).
    /// </summary>
    public string SpaceName { get; set; } = String.Empty;

    #endregion

    #region IFileConnection

    /// <summary>
    /// Attempts to create a new folder.
    /// </summary>
    /// <param name="path">Relative path for the new folder.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the create was successful, other false.</returns>
    public override async Task CreateFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        // create client
        using var client = CreateClient();

        var request = new PutObjectRequest
        {
            BucketName = SpaceName,
            Key = path.EnsureEndsWith(Constants.PATH_SEPARATOR),
            CannedACL = S3CannedACL.Private // is default but here to be explicit
        };
        var result = await client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the delete was successful, other false.</returns>
    public async override Task<bool> DeleteFileAsync(string key, CancellationToken cancellationToken = default)
    {
        // create client
        using var client = CreateClient();

        if (key.EndsWith(Constants.PATH_SEPARATOR))
        {
            // delete folder - fetch all files to be deleted
            var request = new ListObjectsV2Request()
            {
                BucketName = SpaceName,
                Prefix = key
            };
            var response = await client.ListObjectsV2Async(request, cancellationToken).ConfigureAwait(false);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var request2 = new DeleteObjectsRequest
                {
                    BucketName = SpaceName,
                    Objects = new List<KeyVersion>(response.S3Objects.Select(x => new KeyVersion { Key = x.Key }))
                };
                var result = await client.DeleteObjectsAsync(request2, cancellationToken).ConfigureAwait(false);
                return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }
        else
        {
            // delete file
            var request = new DeleteObjectRequest
            {
                BucketName = SpaceName,
                Key = key,
            };
            var result = await client.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
            return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        return false;
    }

    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns></returns>
    public override async Task<Stream> GetFileAsync(string key, CancellationToken cancellationToken = default)
    {
        // create client
        using var client = CreateClient();

        // download file
        var request = new GetObjectRequest
        {
            BucketName = SpaceName,
            Key = key
        };
        var response = await client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false);
        return response.ResponseStream;
    }

    /// <summary>
    /// Attempts to list direcotry entries from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the directory to list.</param>
    /// <param name="cancellationToken">Token to allow cancellation op operation.</param>
    /// <returns>An array of DirectoryEntry instances.</returns>
    public override async Task<DirectoryEntry[]> ListAsync(string path, CancellationToken cancellationToken = default)
    {
        var items = new List<DirectoryEntry>();
        path = path.EnsureEndsWith(Constants.PATH_SEPARATOR);

        // build and send request
        using var client = CreateClient();
        var request = new ListObjectsV2Request()
        {
            BucketName = SpaceName,
            Prefix = path
        };
        var response = await client.ListObjectsV2Async(request, cancellationToken).ConfigureAwait(false);

        // parse response if valid
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            var folders = new List<string>();
            foreach (var s3obj in response.S3Objects)
            {
                var key = s3obj.Key;
                var followingPath = key[path.Length..];
                // sub-folder file?
                if (string.IsNullOrWhiteSpace(followingPath))
                {
                    // ignore own folder entry
                }
                else if (followingPath.Contains(Constants.PATH_SEPARATOR))
                {
                    var folder = followingPath[..followingPath.IndexOf(Constants.PATH_SEPARATOR)];
                    if (!string.IsNullOrWhiteSpace(folder) && !folders.Contains(folder))
                    {
                        folders.Add(folder);
                        items.Add(new DirectoryEntry
                        {
                            Path = path + folder,
                            LastModified = s3obj.LastModified.ToUniversalTime(),
                            Type = DirectoryEntryTypes.Folder
                        });
                    }
                }
                else
                {
                    items.Add(new DirectoryEntry
                    {
                        Path = key,
                        LastModified = s3obj.LastModified,
                        SizeBytes = s3obj.Size,
                        Type = DirectoryEntryTypes.File
                    });
                }
            }
        }

        return items.ToArray();
    }

    /// <summary>
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    public override async Task<bool> PutFileAsync(string key, Stream content, IEnumerable<Metadata>? metadata = null, CancellationToken cancellationToken = default)
    {
        // create client
        using var client = CreateClient();

        // upload file
        var request = new PutObjectRequest
        {
            BucketName = SpaceName,
            Key = key,
            ContentType = "application/octet-stream",
            InputStream = content,
            CannedACL = S3CannedACL.Private // is default but here to be explicit
        };
        if (metadata != null)
        {
            foreach (var md in metadata)
            {
                request.Metadata.Add(md.Name, md.Value);
            }
        }
        var result = await client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    #endregion

    private AmazonS3Client CreateClient()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = ServiceUrl
        };
        return new AmazonS3Client(AccessKey, Secret, config);
    }
}
