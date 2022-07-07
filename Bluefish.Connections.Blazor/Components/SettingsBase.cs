using System.Linq.Expressions;
using System.Reflection;

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
    public async Task OnValueChanged<TValue>(Expression<Func<TConnection, TValue>> member, TValue value)
    {
        var memberSelectorExpression = member.Body as MemberExpression;
        if (memberSelectorExpression != null)
        {
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(_connection, value, null);
            }
        }
        await UpdateSettings().ConfigureAwait(true);
    }

    protected async Task UpdateSettings()
    {
        var settings = JsonSerializer.Serialize(_connection);
        await SettingsChanged.InvokeAsync(settings).ConfigureAwait(true);
    }

}
