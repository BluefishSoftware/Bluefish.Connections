namespace Bluefish.Connections.Blazor.Components;

public partial class OdbcSettings
{
    private async Task OnDriverChanged(string value)
    {
        _connection.Driver = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnDsnChanged(string value)
    {
        _connection.Dsn = value;
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
}
