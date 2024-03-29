﻿using Bluefish.Connections.Models;
using System.Reflection;

namespace Bluefish.Connections.Blazor.Components;

public partial class ConnectionSelector
{
    public const string TOKEN_NONE = "";
    private readonly IDictionary<string, IConnection> _dataTypes = new Dictionary<string, IConnection>();

    [Parameter]
    public Assembly? AdditionalAssembly { get; set; }

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
        // standard connections
        ScanAssembly(typeof(IConnection).Assembly);
        if (AdditionalAssembly != null)
        {
            ScanAssembly(AdditionalAssembly);
        }
        if (!ShowNone && !_dataTypes.ContainsKey(Type) && _dataTypes.Count > 0)
        {
            await OnTypeChanged(_dataTypes.Keys.First());
        }
    }

    private async Task OnTypeChanged(string value)
    {
        Type = value;
        await TypeChanged.InvokeAsync(value).ConfigureAwait(true);
    }

    private void ScanAssembly(Assembly assembly)
    {
        var connectionType = typeof(IConnection);
        Type[] connectionTypes = assembly
            .GetTypes()
            .Where(p => connectionType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
            .ToArray();
        foreach (Type t in connectionTypes)
        {
            if (Activator.CreateInstance(t) is IConnection c && t.Assembly.FullName != null)
            {
                if (FilterType is null
                    || (FilterType == ConnectionTypes.File && t.IsAssignableTo(typeof(IFileConnection)))
                    || (FilterType == ConnectionTypes.Email && t.IsAssignableTo(typeof(IEmailConnection)))
                    || (FilterType == ConnectionTypes.SQL && t.IsAssignableTo(typeof(ISqlConnection))))
                {
                    _dataTypes.Add($"{t.FullName}, {t.Assembly.FullName.Substring(0, t.Assembly.FullName.IndexOf(','))}", c);
                }
            }
        }

    }
}
