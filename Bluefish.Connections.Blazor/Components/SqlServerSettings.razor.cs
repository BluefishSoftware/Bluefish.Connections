namespace Bluefish.Connections.Blazor.Components;

public partial class SqlServerSettings
{
    private SqlServerConnection _connection = new();

    [Parameter]
    public string Settings { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<SqlServerConnection>(Settings) ?? new();
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

    private async Task OnDatabaseChanged(string value)
    {
        _connection.Database = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    //private async Task OnUseWindowsAuthenticationChanged(bool value)
    //{
    //    _connection.UseWindowsAuthentication = value;
    //    await UpdateSettings().ConfigureAwait(true);
    //}

    private async Task OnUserIdChanged(string value)
    {
        _connection.UserId = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnPasswordChanged(string value)
    {
        _connection.Password = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnConnectionTimeoutChanged(int value)
    {
        _connection.ConnectionTimeout = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
