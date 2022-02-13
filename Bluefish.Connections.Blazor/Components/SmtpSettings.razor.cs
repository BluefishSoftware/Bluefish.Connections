namespace Bluefish.Connections.Blazor.Components;

public partial class SmtpSettings
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

    private async Task OnUsernameChanged(string value)
    {
        _connection.Username = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPasswordChanged(string value)
    {
        _connection.Password = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
