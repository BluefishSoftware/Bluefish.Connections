using System.Text;

namespace Bluefish.Connections.Models;

public abstract class SqlConnectionBase : ISqlConnection
{
    protected SqlConnectionBase(string name, string provider)
    {
        Name = name;
        Provider = provider;
    }

    [JsonIgnore]
    public string Name { get; }

    [JsonIgnore]
    public string Provider { get; }

    [JsonIgnore]
    public ConnectionTypes Type => ConnectionTypes.SQL;

    public abstract string GetConnectionString();

    public virtual string GetDataType(Type type, int? maxSize = null, int? precision = 18, int? scale = 2)
    {
        if (type.Equals(typeof(int)))
        {
            return "INT";
        }
        if (type.Equals(typeof(string)))
        {
            return $"NVARCHAR({(maxSize.HasValue ? maxSize : "MAX")})";
        }
        if (type.Equals(typeof(Guid)))
        {
            return "UNIQUEIDENTIFIER";
        }
        if (type.Equals(typeof(decimal)))
        {
            return $"DECIMAL({precision},{scale})";
        }
        if (type.Equals(typeof(DateTime)))
        {
            return $"DATETIME";
        }
        if (type.Equals(typeof(DateTimeOffset)))
        {
            return $"DATETIMEOFFSET";
        }
        if (type.Equals(typeof(bool)))
        {
            return "BIT";
        }
        if (type.Equals(typeof(Int64)))
        {
            return "BIGINT";
        }
        if (type.Equals(typeof(Int16)))
        {
            return "SMALLINT";
        }
        return String.Empty;
    }

    public abstract DbProviderFactory GetFactory();

    public virtual string GetIdentitySpecifier()
    {
        return "IDENTITY";
    }

    public virtual string GetPagedQuery(string query, int take = 0, int skip = 0)
    {
        query = query.Trim();
        var terminated = query.EndsWith(';');
        var multiline = query.Contains('\n');
        var sb = new StringBuilder(query.TrimEnd(';'));
        if (multiline)
        {
            sb.AppendLine();
        }
        if (skip > 0 || take > 0)
        {
            if(multiline)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(' ');
            }
            sb.Append($"OFFSET {skip} ROWS");
            if (multiline)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(' ');
            }
            sb.Append($"FETCH NEXT {take} ROWS ONLY");
        }
        if (terminated)
        {
            sb.Append(';');
        }
        return sb.ToString();
    }

    public virtual string GetParameterPlaceholder(string name)
    {
        return $"@{name}";
    }

    public virtual string GetQuotedValue(string value)
    {
        return $"{GetQuotePrefix()}{value}{GetQuoteSuffix()}";
    }

    public virtual string GetQuotePrefix()
    {
        return "[";
    }

    public virtual string GetQuoteSuffix()
    {
        return "]";
    }
}
