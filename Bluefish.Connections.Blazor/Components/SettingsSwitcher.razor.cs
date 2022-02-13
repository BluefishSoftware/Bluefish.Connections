namespace Bluefish.Connections.Blazor.Components;

public partial class SettingsSwitcher
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
    public RenderFragment? ChildContent { get; set; }

}
