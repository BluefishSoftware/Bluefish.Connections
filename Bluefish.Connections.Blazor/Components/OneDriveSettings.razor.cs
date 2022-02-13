namespace Bluefish.Connections.Blazor.Components;

public partial class OneDriveSettings
{
    private OneDriveConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<OneDriveConnection>(Settings) ?? new();
        }
    }

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

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
