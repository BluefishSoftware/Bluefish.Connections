namespace Bluefish.Connections.Blazor.Components;

public partial class PostgreSqlSettings
{
    private async Task OnHostChanged(string value)
    {
        _connection.Host = value;
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

    private async Task OnSslModeChanged(string value)
    {
        _connection.SslMode = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
