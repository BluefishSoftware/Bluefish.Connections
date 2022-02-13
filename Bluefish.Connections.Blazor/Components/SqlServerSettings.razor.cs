namespace Bluefish.Connections.Blazor.Components;

public partial class SqlServerSettings
{
    private async Task OnServerChanged(string value)
    {
        _connection.Server = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPortChanged(int value)
    {
        _connection.Port = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnDatabaseChanged(string value)
    {
        _connection.Database = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnUseWindowsAuthenticationChanged(bool value)
    {
        _connection.UseWindowsAuthentication = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnUserIdChanged(string value)
    {
        _connection.UserId = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPasswordChanged(string value)
    {
        _connection.Password = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnConnectionTimeoutChanged(int value)
    {
        _connection.ConnectionTimeout = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
