namespace Bluefish.Connections.Blazor.Components;

public partial class LocalFileSystemSettings
{
    private async Task OnRootPathChanged(string value)
    {
        _connection.RootPath = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
