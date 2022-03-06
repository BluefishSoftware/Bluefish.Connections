using Bluefish.Connections.Models;

namespace Bluefish.Connections.Blazor.Components;

public partial class ConnectionSelector
{
    public const string TOKEN_NONE = "";
    private readonly IDictionary<string, string> _dataTypes = new Dictionary<string, string>();

    [Parameter]
    public bool CanEdit { get; set; } = true;

    [Parameter]
    public string CssClass { get; set; } = "mb-2";

    [Parameter]
    public ConnectionTypes? FilterType { get; set; }

    [Parameter]
    public bool ShowNone { get; set; } = false;

    [Parameter]
    public string Type { get; set; } = TOKEN_NONE;

    [Parameter]
    public EventCallback<string> TypeChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var connectionType = typeof(IConnection);
        Type[] connectionTypes = typeof(IConnection).Assembly
            .GetTypes()
            .Where(p => connectionType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
            .ToArray();
        foreach (Type t in connectionTypes)
        {
            if (Activator.CreateInstance(t) is IConnection c && t.Assembly.FullName != null)
            {
                if(FilterType is null
                    || (FilterType == ConnectionTypes.File && t.IsAssignableTo(typeof(IFileConnection)))
                    || (FilterType == ConnectionTypes.Email && t.IsAssignableTo(typeof(IEmailConnection)))
                    || (FilterType == ConnectionTypes.SQL && t.IsAssignableTo(typeof(ISqlConnection))))
                {
                    _dataTypes.Add($"{t.FullName}, {t.Assembly.FullName.Substring(0, t.Assembly.FullName.IndexOf(','))}", c.Name);
                }
            }
        }
        if(!ShowNone && _dataTypes.Count > 0)
        {
            await OnTypeChanged(_dataTypes.Keys.First());
        }
    }

    private async Task OnTypeChanged(string value)
    {
        Type = value;
        await TypeChanged.InvokeAsync(value).ConfigureAwait(true);
    }
}
