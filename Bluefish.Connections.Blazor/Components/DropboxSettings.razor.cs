namespace Bluefish.Connections.Blazor.Components;

public partial class DropboxSettings
{
    private async Task OnAppKeyChanged(string value)
    {
        _connection.AppKey = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnAppSecretChanged(string value)
    {
        _connection.AppSecret = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnAccessTokenChanged(string value)
    {
        _connection.AccessToken = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnRootFolderChanged(string value)
    {
        _connection.RootFolder = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
