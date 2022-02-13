namespace Bluefish.Connections.Blazor.Components;

public partial class FtpSettings
{
    private async Task OnFtpHostChanged(string value)
    {
        _connection.FtpHost = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnFtpPasswordChanged(string value)
    {
        _connection.FtpPassword = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnFtpPortChanged(int value)
    {
        _connection.FtpPort = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnFtpTimeoutChanged(int value)
    {
        _connection.FtpTimeout = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnFtpUsernameChanged(string value)
    {
        _connection.FtpUsername = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnProxyPasswordChanged(string value)
    {
        _connection.ProxyPassword = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnProxyPortChanged(int value)
    {
        _connection.ProxyPort = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnProxyServerChanged(string value)
    {
        _connection.ProxyServer = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnProxyTypeChanged(FtpProxyServerTypes value)
    {
        _connection.ProxyType = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnProxyUserChanged(string value)
    {
        _connection.ProxyUser = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnRootFolderChanged(string value)
    {
        _connection.RootFolder = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnSslModeChanged(FtpSslModes value)
    {
        _connection.SslMode = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
