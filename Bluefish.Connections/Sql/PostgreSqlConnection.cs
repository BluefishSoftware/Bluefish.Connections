using Npgsql;
using System.Text;

namespace Bluefish.Connections.Sql;

/// <summary>
/// The PostgreSqlConnection class represents a connection to a PostgreSql database.
/// </summary>
public class PostgreSqlConnection : SqlConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the PostgreSqlConnection class.
    /// </summary>
    public PostgreSqlConnection()
        : base("PostgreSql", "Npgsql")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the hostname or IP address of the SQL Server.
    /// </summary>
    public string Host { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the port number of the SQL Server.
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Gets or sets the database on the server to connect with.
    /// </summary>
    public string Database { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the User Id of account to connect as, leave blank to use integrated security.
    /// </summary>
    public string UserId { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the password for the account to connect as.
    /// </summary>
    public string Password { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets whether connection pooling is enabled.
    /// </summary>
    public bool Pooling { get; set; } = true;

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 10;

    /// <summary>
    /// Gets or sets the SSL mode to connect with.
    /// </summary>
    public string SslMode { get; set; } = "Prefer";

    #endregion

    #region ISqlConnection

    public override string GetConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Pooling = Pooling,
            Username = UserId,
            Password = Password,
            Timeout = ConnectionTimeout,
            SslMode = (SslMode)Enum.Parse(typeof(SslMode), SslMode)
        };
        return builder.ToString();
    }

    public override string GetDataType(Type type, int? maxSize = null, int? precision = 18, int? scale = 2)
    {
        if (type.Equals(typeof(string)))
        {
            return maxSize.HasValue ? $"VARCHAR({maxSize})" : "TEXT";
        }
        if (type.Equals(typeof(Guid)))
        {
            return "UUID";
        }
        if (type.Equals(typeof(DateTime)))
        {
            return $"TIMESTAMP";
        }
        if (type.Equals(typeof(DateTimeOffset)))
        {
            return $"TIMESTAMPTZ";
        }
        if (type.Equals(typeof(bool)))
        {
            return "BOOLEAN";
        }
        return base.GetDataType(type, maxSize, precision, scale);
    }

    public override DbProviderFactory GetFactory()
    {
        return NpgsqlFactory.Instance;
    }

    public override string GetIdentitySpecifier()
    {
        return "GENERATED ALWAYS AS IDENTITY";
    }

    public override string GetPagedQuery(string query, int take = 0, int skip = 0)
    {
        query = query.Trim();
        var terminated = query.EndsWith(';');
        var multiline = query.Contains('\n');
        var sb = new StringBuilder(query.TrimEnd(';'));
        if (multiline)
        {
            sb.AppendLine();
        }
        if (take > 0)
        {
            if (multiline)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(' ');
            }
            sb.Append($"LIMIT {take}");
        }
        if (skip > 0)
        {
            if (multiline)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(' ');
            }
            sb.Append($"OFFSET {skip}");
        }
        if(terminated)
        {
            sb.Append(';');
        }
        return sb.ToString();
    }


    public override string GetQuotePrefix()
    {
        return "\"";
    }

    public override string GetQuoteSuffix()
    {
        return "\"";
    }

    #endregion
}
