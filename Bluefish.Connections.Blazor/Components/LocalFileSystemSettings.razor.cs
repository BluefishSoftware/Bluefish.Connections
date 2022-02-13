namespace Bluefish.Connections.Blazor.Components;

public partial class LocalFileSystemSettings
{
    private LocalFileSystemConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<LocalFileSystemConnection>(Settings) ?? new();
        }
    }

    private async Task OnRootPathChanged(string value)
    {
        _connection.RootPath = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
