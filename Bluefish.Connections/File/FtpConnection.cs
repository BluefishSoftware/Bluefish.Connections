using Rebex;
using Rebex.IO;
using Rebex.Net;

namespace Bluefish.Connections.File;

public class FtpConnection : FileConnectionBase, IInitializableConnection
{
    /// <summary>
    /// Initializes a new instance of the FtpConnection class.
    /// </summary>
    public FtpConnection()
        : base("FTP")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the host name or IP address of the FTP server.
    /// </summary>
    public string FtpHost { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the port number of the FTP server.
    /// </summary>
    public int FtpPort { get; set; } = 21;

    /// <summary>
    /// Gets or sets the username to log in with.
    /// </summary>
    public string FtpUsername { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the password to log in with.
    /// </summary>
    public string FtpPassword { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the path of the root folder.
    /// </summary>
    public string RootFolder { get; set; } = String.Empty;

    public FtpSslModes SslMode { get; set; }

    /// <summary>
    /// Gets or sets the timeout of commands, in milliseconds.
    /// </summary>
    public int FtpTimeout { get; set; } = 60000;

    /// <summary>
    /// Gets or sets the type of the proxy server.
    /// </summary>
    public FtpProxyServerTypes ProxyType { get; set; }

    /// <summary>
    /// Gets or sets the IP or name of the proxy server.
    /// </summary>
    public string ProxyServer { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the port number of the proxy server.
    /// </summary>
    public int ProxyPort { get; set; } = 21;

    /// <summary>
    /// Gets or sets the user name of the account to connect to the proxy server as.
    /// </summary>
    public string ProxyUser { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the password of the account to connect to proxy server as.
    /// </summary>
    public string ProxyPassword { get; set; } = String.Empty;

    #endregion

    #region IInitializableConnection

    /// <summary>
    /// Initializes the connection.
    /// </summary>
    /// <param name="configuration">Configuration instance to obtain settings from.</param>
    public void Initialize(IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(Licensing.Key) && configuration != null)
        {
            // This key will work for all builds of Rebex FTP/SSL released before 2021-07-01
            var key = configuration["RebexKey"];
            if (!string.IsNullOrWhiteSpace(key))
            {
                Licensing.Key = key;
            }
        }
    }

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
        using var client = new Ftp();
        ConnectToServer(client);
        await client.DeleteFileAsync(path).ConfigureAwait(false);
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
        using var client = new Ftp();
        ConnectToServer(client);

        // descend into folder containing recording
        var fullPath = $"{RootFolder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";
        var parts = fullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < parts.Length - 1; i++)
        {
            await DescendIntoFolderAsync(parts[i], client).ConfigureAwait(false);
        }

        // download file to temp folder
        var tempFolder = Path.GetTempPath();
        var tempPath = Path.Combine(tempFolder, Path.GetFileName(path));
        try
        {
            await client.DownloadAsync(parts.Last(), tempFolder, TraversalMode.Recursive, TransferMethod.Copy, ActionOnExistingFiles.ResumeIfPossible).ConfigureAwait(false);

            // open file and return stream
            return System.IO.File.Open(tempPath, FileMode.Open);
        }
        catch
        {
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }
        }
        return Stream.Null;
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
        using (var client = new Ftp())
        {
            var tempFolder = Path.GetTempPath();
            var tempPath = Path.Combine(tempFolder, Path.GetFileName(path));
            var fullPath = $"{RootFolder.Replace('\\', '/').TrimEnd('/')}/{path.Replace('\\', '/').TrimStart('/')}";

            try
            {
                ConnectToServer(client);

                // save file to temp folder
                using FileStream tempStream = System.IO.File.Open(tempPath, FileMode.Create);
                await content.CopyToAsync(tempStream, cancellationToken).ConfigureAwait(false);
                tempStream.Dispose();

                // descend into folder containing recording
                var parts = fullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < parts.Length - 1; i++)
                {
                    await DescendIntoFolderAsync(parts[i], client).ConfigureAwait(false);
                }

                // upload file
                await client.UploadAsync(tempPath,
                                         ".",
                                         TraversalMode.Recursive,
                                         TransferMethod.Copy,
                                         ActionOnExistingFiles.OverwriteAll).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                // remove temp files
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }
    }

    #endregion

    private void ConnectToServer(Ftp client)
    {
        // set timeout in ms
        client.Timeout = FtpTimeout;

        // use proxy server
        if (!string.IsNullOrWhiteSpace(ProxyServer))
        {
            //Logger?.Debug("Setting up {type} proxy for FTP: {host:port}", ProxyType.ToString(), ProxyServer, ProxyPort);
            client.Proxy.ProxyType = (FtpProxyType)ProxyType;
            client.Proxy.Host = ProxyServer;
            client.Proxy.Port = ProxyPort;
            if (!string.IsNullOrWhiteSpace(ProxyUser))
            {
                client.Proxy.UserName = ProxyUser;
                client.Proxy.Password = ProxyPassword;
            }
        }

        //Logger?.Debug("Connecting to FTP server {host}", Host);
        client.Settings.SslAllowedSuites = TlsCipherSuite.Fast; // important - if omitted 20-60 seconds handshake!
        client.Connect(FtpHost, FtpPort, (SslMode)SslMode);
        client.Login(FtpUsername, FtpPassword);
    }

    private static async Task DescendIntoFolderAsync(string folder, Rebex.Net.Ftp client)
    {
        if (!client.DirectoryExists(folder))
        {
            await client.CreateDirectoryAsync(folder).ConfigureAwait(false);
        }
        await client.ChangeDirectoryAsync(folder).ConfigureAwait(false);
    }
}

public enum FtpProxyServerTypes
{
    //
    // Summary:
    //     Direct connection.
    None = 0,
    //
    // Summary:
    //     Connection through Socks4 proxy.
    Socks4 = 1,
    //
    // Summary:
    //     Connection through Socks4A proxy (capable of resolving domain names).
    Socks4a = 2,
    //
    // Summary:
    //     Connection through Socks5 proxy.
    Socks5 = 3,
    //
    // Summary:
    //     Connection through HTTP proxy using CONNECT method.
    HttpConnect = 4,
    //
    // Summary:
    //     Connection through FTP proxy using SITE command.
    FtpSite = 5,
    //
    // Summary:
    //     Connection through FTP proxy using a single USER/PASS command.
    FtpUser = 6,
    //
    // Summary:
    //     Connection through FTP proxy using OPEN command.
    FtpOpen = 7,
    //
    // Summary:
    //     Connection through FTP proxy using two separate USER/PASS commands (one USER
    //     command for logging in the proxy and the second USER command for logging in the
    //     FTP server).
    FtpDoubleLogin = 8
}

public enum FtpSslModes
{
    //
    // Summary:
    //     Unsecured connection. No TLS/SSL.
    None = 0,
    //
    // Summary:
    //     Implicitly secured TLS/SSL connection.
    Implicit = 1,
    //
    // Summary:
    //     Explicitly secured TLS/SSL connection.
    Explicit = 2
}
