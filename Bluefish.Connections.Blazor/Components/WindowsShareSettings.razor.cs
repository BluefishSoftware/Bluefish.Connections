namespace Bluefish.Connections.Blazor.Components;

public partial class WindowsShareSettings
{
    private WindowsShareConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<WindowsShareConnection>(Settings) ?? new();
        }
    }

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

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
