namespace Bluefish.Connections.Blazor.Components;

public abstract class SettingsBase<TConnection> : ComponentBase where TConnection : new()
{
    protected TConnection _connection = new();

    [Parameter]
    public bool CanEdit { get; set; } = true;

    [Parameter]
    public string CssClass { get; set; } = "mb-2";

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Settings))
        {
            _connection = JsonSerializer.Deserialize<TConnection>(Settings) ?? new();
        }
    }

    protected async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
