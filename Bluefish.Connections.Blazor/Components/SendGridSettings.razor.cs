namespace Bluefish.Connections.Blazor.Components;

public partial class SendGridSettings
{
    private async Task OnApiKeyChanged(string value)
    {
        _connection.ApiKey = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
