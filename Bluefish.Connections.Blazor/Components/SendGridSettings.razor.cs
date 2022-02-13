namespace Bluefish.Connections.Blazor.Components;

public partial class SendGridSettings
{
    private SendGridConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }
    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<SendGridConnection>(Settings) ?? new();
        }
    }

    private async Task OnApiKeyChanged(string value)
    {
        _connection.ApiKey = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
