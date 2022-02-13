namespace Bluefish.Connections.Blazor.Components;

public partial class WindowsShareSettings
{
    private async Task OnRootPathChanged(string value)
    {
        _connection.RootPath = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnServerNameChanged(string value)
    {
        _connection.ServerName = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnShareNameChanged(string value)
    {
        _connection.ShareName = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
