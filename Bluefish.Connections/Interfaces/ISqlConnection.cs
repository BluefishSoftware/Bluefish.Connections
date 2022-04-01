namespace Bluefish.Connections.Interfaces;

/// <summary>
/// The ISqlConnection interface defines the required properties and method for a SQL Connection.
/// </summary>
public interface ISqlConnection : IConnection
{
    string GetConnectionString();
    string GetDataType(Type type, int? maxSize = null, int? precision = 18, int? scale = 2);
    string GetParameterPlaceholder(string name);
    string Provider { get; }
    DbProviderFactory GetFactory();
    string GetIdentitySpecifier();
    string GetQuotedValue(string value);
    string GetQuotePrefix();
    string GetQuoteSuffix();
}
