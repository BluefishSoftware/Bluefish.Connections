namespace Bluefish.Connections.Blazor.Components;

public partial class DropboxSettings
{
    private DropboxConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<DropboxConnection>(Settings) ?? new ();
        }
    }

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

    private async Task UpdateSettings()
    {
        var settings = System.Text.Json.JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
