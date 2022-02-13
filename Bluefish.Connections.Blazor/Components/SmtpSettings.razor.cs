namespace Bluefish.Connections.Blazor.Components;

public partial class SmtpSettings
{
    private SmtpConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }
    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<SmtpConnection>(Settings) ?? new();
        }
    }

    private async Task OnServerChanged(string value)
    {
        _connection.Server = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPortChanged(int value)
    {
        _connection.Port = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnUsernameChanged(string value)
    {
        _connection.Username = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPasswordChanged(string value)
    {
        _connection.Password = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
