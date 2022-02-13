namespace Bluefish.Connections.Blazor.Components;

public partial class DigitalOceanSpacesSettings
{
    private async Task OnServiceUrlChanged(string value)
    {
        _connection.ServiceUrl = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnSpaceNameChanged(string value)
    {
        _connection.SpaceName = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnAccessKeyChanged(string value)
    {
        _connection.AccessKey = value;
        await UpdateSettings().ConfigureAwait(true);
    }

    private async Task OnSecretChanged(string value)
    {
        _connection.Secret = value;
        await UpdateSettings().ConfigureAwait(true);
    }
}
