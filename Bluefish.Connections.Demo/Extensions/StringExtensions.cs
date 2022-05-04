using System.Text.Json;

namespace Bluefish.Connections.Demo.Extensions;

public static class StringExtensions
{
    public static T? InstantiateConnection<T>(this string connectionType, string settings) where T : class
    {
        if (string.IsNullOrWhiteSpace(connectionType))
        {
            return default;
        }
        // instantiate connection
        var t = Type.GetType(connectionType);
        if (t is null)
        {
            throw new Exception("Invalid Data Type for File Storage connection.");
        }
        return (T?)JsonSerializer.Deserialize(settings, t);
    }
}
