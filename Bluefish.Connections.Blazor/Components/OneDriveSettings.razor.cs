namespace Bluefish.Connections.Blazor.Components;

public partial class OneDriveSettings
{
    private async Task OnAccessTokenChanged(string value)
    {
        _connection.AccessToken = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnClientIdChanged(string value)
    {
        _connection.ClientId = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnClientSecretChanged(string value)
    {
        _connection.ClientSecret = value;
        await UpdateSettings().ConfigureAwait(true);
    }
    private async Task OnDriveChanged(string value)
    {
        _connection.Drive = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnFolderChanged(string value)
    {
        _connection.Folder = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnTenantIdChanged(string value)
    {
        _connection.TenantId = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnTokenExpiryChanged(string value)
    {
        if (DateTime.TryParse(value, out DateTime expiry))
        {
            _connection.TokenExpiry = expiry;
            await UpdateSettings().ConfigureAwait(true);
        }
    }
}
