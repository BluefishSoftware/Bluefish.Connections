namespace Bluefish.Connections.Blazor.Components;

public partial class ConnectionSettings
{
    [Parameter]
    public bool CanEdit { get; set; } = true;

    [Parameter]
    public string ConnectionType { get; set; } = typeof(LocalFileSystemConnection).FullName ?? String.Empty;

    [Parameter]
    public string CssClass { get; set; } = "mb-2";

    [Parameter]
    public string Settings { get; set; } = String.Empty;

    [Parameter]
    public EventCallback<string> SettingsChanged { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task OnSettingsChanged(string settings)
    {
        Settings = settings;
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }
}
