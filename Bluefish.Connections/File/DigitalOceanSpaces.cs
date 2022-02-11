using Amazon.S3;
using Amazon.S3.Model;

namespace Bluefish.Connections.File;

/// <summary>
/// The DigitalOceanSpaces class implements a connection that stores and retrieves files to
/// a DigitalOcean Spaces account.
/// </summary>
public class DigitalOceanSpaces : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the DigitalOceanSpaces class.
    /// </summary>
    public DigitalOceanSpaces()
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
    /// Attempts to delete the given file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the delete was successful, other false.</returns>
    public async override Task<bool> DeleteFileAsync(string key, CancellationToken cancellationToken = default)
    {
        // create client
        using var client = CreateClient();

        // delete file
        var request = new DeleteObjectRequest
        {
            BucketName = SpaceName,
            Key = key
        };
        var result = await client.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
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
    /// Attempts to save the given file to DigitalOcean Spaces.
    /// </summary>
    /// <param name="key">Unique identifier within Space for file.</param>
    /// <param name="content">Actual data top be saved.</param>
    /// <param name="metadata">Optional metadata to store against the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>true if the upload was successful, other false.</returns>
    public override async Task<bool> PutFileAsync(string key, Stream content, IEnumerable<NameValue>? metadata = null, CancellationToken cancellationToken = default)
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
