using System.Data.Odbc;
using System.Text;

namespace Bluefish.Connections.Sql;

/// <summary>
/// The OdbcConnection class represents a connection to an ODBC datasource.
/// </summary>
public class OdbcConnection : SqlConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the SqlServerConnection class.
    /// </summary>
    public OdbcConnection()
        : base("ODBC", "System.Data.SqlClient")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the data source name (DSN).
    /// </summary>
    public string Dsn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the driver to use.
    /// </summary>
    public string Driver { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    #endregion

    #region ISqlConnection

    public override string GetConnectionString()
    {
        var builder = new StringBuilder();
        if(!string.IsNullOrWhiteSpace(Driver))
        {
            builder.Append($"driver={Driver};");
        }
        if (!string.IsNullOrWhiteSpace(Dsn))
        {
            builder.Append($"dsn={Dsn};");
        }
        if (!string.IsNullOrWhiteSpace(UserId))
        {
            builder.Append($"uid={UserId};");
        }
        if (!string.IsNullOrWhiteSpace(Password))
        {
            builder.Append($"pwd={Password};");
        }
        return builder.ToString();
    }

    public override DbProviderFactory GetFactory()
    {
        return OdbcFactory.Instance;
    }

    #endregion
}
