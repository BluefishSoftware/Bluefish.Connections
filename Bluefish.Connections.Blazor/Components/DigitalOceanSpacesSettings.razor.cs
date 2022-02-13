namespace Bluefish.Connections.Blazor.Components;

public partial class DigitalOceanSpacesSettings
{
    private DigitalOceanSpacesConnection _connection = new();

    [Parameter]
    public bool CanEdit { get; set; } = true;


    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<DigitalOceanSpacesConnection>(Settings) ?? new();
        }
    }

    private async Task OnServiceUrlChanged(string value)
    {
        _connection.ServiceUrl = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnSpaceNameChanged(string value)
    {
        _connection.SpaceName = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnAccessKeyChanged(string value)
    {
        _connection.AccessKey = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnSecretChanged(string value)
    {
        _connection.Secret = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
