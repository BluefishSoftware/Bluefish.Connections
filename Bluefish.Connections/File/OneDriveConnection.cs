using System.Net;
using System.Text;

namespace Bluefish.Connections.File;

public class OneDriveConnection : FileConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the OneDriveConnection class.
    /// </summary>
    public OneDriveConnection()
        : base("Microsoft OneDrive")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets client id.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique Tenant id for the customer.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC date and time that the token will expire at.
    /// </summary>
    public DateTime? TokenExpiry { get; set; }

    /// <summary>
    /// Gets or sets the drive where recordings are to be stored.
    /// </summary>
    public string Drive { get; set; } = "/drive/root";

    /// <summary>
    /// Gets or sets the folder where files are moved to.
    /// </summary>
    public string Folder { get; set; } = string.Empty;

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
        return Task.FromResult(true);
    }


    /// <summary>
    /// Attempts to load a file from DigitalOcean Spaces.
    /// </summary>
    /// <param name="path">Relative path for the file.</param>
    /// <param name="cancellationToken">Token to allow cancellation of save.</param>
    /// <returns>A Stream with file contents.</returns>
    public override async Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // get new token if a) no token or b) within 5 minutes of token expiry
        if (string.IsNullOrWhiteSpace(AccessToken) || TokenExpiry.HasValue && DateTime.UtcNow > TokenExpiry.Value.AddMinutes(-5))
        {
            await GetNewAccessTokenAsync().ConfigureAwait(false);
        }

        // if here then should have a valid access token
        HttpResponseMessage? response = null;
        using (HttpClient client = new())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            var fullPath = $"{Folder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
            var drive = string.IsNullOrWhiteSpace(Drive) ? "/drive/root" : Drive.TrimStart('/');
            var url = $"https://graph.microsoft.com/v1.0/{drive}:{fullPath}:/content";
            response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        // error
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, new JsonSerializerOptions { WriteIndented = true });
        if (string.IsNullOrWhiteSpace(errorResponse!.Error.Code))
        {
            throw new Exception($"Upload failed: {json}");
        }
        throw new Exception($"Upload failed: {errorResponse.Error.Code}: {errorResponse.Error.Message}");
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
        var tempFolder = Path.GetTempPath();

        // get new token if a) no token or b) within 5 minutes of token expiry
        if (string.IsNullOrWhiteSpace(AccessToken) || TokenExpiry.HasValue && DateTime.UtcNow > TokenExpiry.Value.AddMinutes(-5))
        {
            await GetNewAccessTokenAsync().ConfigureAwait(false);
        }

        // if here then should have a valid access token
        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
        var drive = string.IsNullOrWhiteSpace(Drive) ? "/drive/root" : Drive.TrimStart('/');
        var fullPath = $"{Folder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
        var recordingUrl = $"https://graph.microsoft.com/v1.0/{drive}:{fullPath}:";
        HttpResponseMessage response = new();


        // if small file - upload in one request
        if (content.Length < 4000000)
        {
            var buffer = new Memory<byte>();
            var bytesRead = await content.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            response = await client.PutAsync($"{recordingUrl}/content", new ByteArrayContent(buffer.ToArray()), cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // upload larger files in segments

            // create session
            var url = $"{recordingUrl}/createUploadSession";
            var stringContent = new StringContent("{ \"item\": { \"@microsoft.graph.conflictBehavior\": \"replace\" } }", Encoding.UTF8, "application/json");
            response = await client.PostAsync(url, stringContent).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var sessionInfo = JsonSerializer.Deserialize<UploadSessionResponse>(response.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { WriteIndented = true });

                // only need to send token on session setup, this avoids possible 401 during upload of many chunks
                client.DefaultRequestHeaders.Authorization = null;

                // divide file up into chunks
                const int chunkSize = 327680 * 10; // MUST be a multiple of 327,680

                var i = 0;
                var buffer = new byte[chunkSize];
                var bytesRead = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                while(bytesRead > 0)
                {
                    var chunkRequest = new HttpRequestMessage(HttpMethod.Put, sessionInfo!.UploadUrl)
                    {
                        Content = new ByteArrayContent(buffer, 0, bytesRead)
                    };
                    chunkRequest.Content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(i, i + bytesRead - 1, bytesRead);
                    response = await client.SendAsync(chunkRequest, cancellationToken).ConfigureAwait(false);
                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        break;
                    }
                    i += bytesRead;
                    bytesRead = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        // should receive 201 if successfully uploaded
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return true;
        }

        // else error
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, new JsonSerializerOptions { WriteIndented = true });
        if (string.IsNullOrWhiteSpace(errorResponse!.Error.Code))
        {
            throw new Exception($"Upload failed: {json}");
        }
        throw new Exception($"Upload failed: {errorResponse.Error.Code}: {errorResponse.Error.Message}");
    }

    private async Task GetNewAccessTokenAsync()
    {
        try
        {
            using HttpClient client = new();
            const string scope = "https://graph.microsoft.com/.default";
            var url = $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token";
            var dict = new Dictionary<string, string>
                    {
                        { "client_id", ClientId },
                        { "scope", scope },
                        { "client_secret", ClientSecret },
                        { "grant_type", "client_credentials" }
                    };
            HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(dict)).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<GetTokenResponse>(json, new JsonSerializerOptions { WriteIndented = true });
            if (!string.IsNullOrWhiteSpace(result!.Error))
            {
                throw new Exception($"{result.Error}: {result.Error_Description}");
            }
            if (string.IsNullOrWhiteSpace(result.Access_Token))
            {
                throw new Exception("No Access Token returned");
            }

            // update settings
            AccessToken = result.Access_Token;
            TokenExpiry = DateTime.UtcNow.AddSeconds(result.Expires_In);
            //SettingsChanged = true;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to obtain new access token", ex);
        }
    }

    #endregion
}

public class ErrorDetails
{
    public string Code { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
    public ErrorDetails Innererror { get; set; } = new();
}

public class ErrorResponse
{
    public ErrorDetails Error { get; set; } = new();
}

public class GetTokenResponse
{
    public string Token_Type { get; set; } = String.Empty;
    public string Access_Token { get; set; } = String.Empty;
    public int Expires_In { get; set; }
    public string Error { get; set; } = String.Empty;
    public string Error_Description { get; set; } = String.Empty;
}

public class UploadSessionResponse
{
    public string UploadUrl { get; set; } = String.Empty;
    public string ExpirationDateTime { get; set; } = String.Empty;
}
