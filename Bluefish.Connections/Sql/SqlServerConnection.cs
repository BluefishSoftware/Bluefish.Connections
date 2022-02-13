using System.Data.SqlClient;

namespace Bluefish.Connections.Sql;

/// <summary>
/// The SqlServerConnection class represents a connection to a SQL Server.
/// </summary>
public class SqlServerConnection : SqlConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the SqlServerConnection class.
    /// </summary>
    public SqlServerConnection()
        : base("SQL Server", "System.Data.SqlClient")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the hostname or IP address of the SQL Server.
    /// </summary>
    public string Server { get; set; } = "(local)\\SQLExpress";

    /// <summary>
    /// Gets or sets the port number of the SQL Server.
    /// </summary>
    public int Port { get; set; } = 1433;

    /// <summary>
    /// Gets or sets the database on the server to connect with.
    /// </summary>
    public string Database { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets whether Windows Authentication is to be used.
    /// </summary>
    public bool UseWindowsAuthentication { get; set; } = true;

    /// <summary>
    /// Gets or sets the User Id of account to connect as.
    /// </summary>
    public string UserId { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the password for the account to connect as.
    /// </summary>
    public string Password { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 10;

    #endregion

    #region ISqlConnection

    public override string GetConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Port == 1433 ? Server ?? "" : $"{Server},{Port}",
            ConnectTimeout = ConnectionTimeout
        };
        builder.InitialCatalog = Database ?? string.Empty;
        if (string.IsNullOrWhiteSpace(UserId))
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            builder.IntegratedSecurity = false;
            builder.UserID = UserId ?? string.Empty;
            builder.Password = Password ?? string.Empty;
        }
        return builder.ToString();
    }

    public override DbProviderFactory GetFactory()
    {
        return SqlClientFactory.Instance;
    }

    #endregion
}
