using Bluefish.Connections.Extensions;
using Bluefish.Connections.Interfaces;
using System.Text;

namespace Bluefish.Connections.Demo.Pages;

public partial class SqlTest
{
    private Model _model = new();

    public class Model
    {
        public string ConnectionType { get; set; } = string.Empty;

        public string ConnectionSettings { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public string Query { get; set; } = string.Empty;

        public int Rows { get; set; }

        public string Output { get; set; } = string.Empty;
    }

    private async Task OnRunQueryAsync()
    {
        try
        {
            _model.Rows = 0;
            _model.Output = string.Empty;
            _model.ErrorMessage = string.Empty;
            var connection = _model.ConnectionType.InstantiateConnection<ISqlConnection>(_model.ConnectionSettings);
            if (connection is null)
            {
                throw new Exception("Failed to instantiate connection");
            }
            var dbFactory = connection.GetFactory();
            using var dbConnection = dbFactory.CreateConnection();
            if (dbConnection is null)
            {
                throw new Exception("Failed to instantiate database connection");
            }
            dbConnection.ConnectionString = connection.GetConnectionString();
            await dbConnection.OpenAsync().ConfigureAwait(true);
            using var dbCommand = dbFactory.CreateCommand();
            if (dbCommand is null)
            {
                throw new Exception("Failed to instantiate database command");
            }
            dbCommand.CommandText = _model.Query;
            dbCommand.Connection = dbConnection;
            using var reader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(true);
            if (reader != null)
            {
                var sb = new StringBuilder();
                while (reader.Read())
                {
                    if (_model.Rows > 0)
                    {
                        sb.AppendLine();
                    }
                    _model.Rows++;
                    for (var idx = 0; idx < reader.FieldCount; idx++)
                    {
                        if (idx > 0)
                        {
                            sb.Append(", ");
                        }
                        var fieldValue = reader[idx];
                        if (fieldValue != null && fieldValue != DBNull.Value)
                        {
                            sb.Append(fieldValue.ToString());
                        }
                    }
                    if (_model.Rows >= 50)
                    {
                        break;
                    }
                }
                _model.Output = sb.ToString();
            }
        }
        catch (Exception ex)
        {
            _model.ErrorMessage = ex.Message;
        }
    }
}
