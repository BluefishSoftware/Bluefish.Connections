using Bluefish.Connections.Extensions;
using Bluefish.Connections.Interfaces;
using Bluefish.Connections.Models;

namespace Bluefish.Connections.Demo.Pages;

public partial class EmailTest
{
    private Model _model = new();

    public class Model
    {
        public string ConnectionType { get; set; } = string.Empty;

        public string ConnectionSettings { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public string To { get; set; } = "";

        public string Cc { get; set; } = "";

        public string Bcc { get; set; } = "";

        public string Body { get; set; } = "This is a test email";

        public string Subject { get; set; } = "Test Email";

        public string ResultMessage { get; set; } = "";
    }

    private async Task OnSendEmailAsync()
    {
        try
        {
            _model.ErrorMessage = string.Empty;
            _model.ResultMessage = string.Empty;
            var connection = _model.ConnectionType.InstantiateConnection<IEmailConnection>(_model.ConnectionSettings);
            if (connection is null)
            {
                throw new Exception("Failed to instantiate connection");
            }

            var content = new EmailContent
            {
                To = _model.To,
                Cc = _model.Cc,
                Bcc = _model.Bcc,
                Subject = _model.Subject,
                HtmlMessage = _model.Body
            };
            await connection.SendEmailAsync(content);

            _model.ResultMessage = $"Email successfully sent at {DateTime.Now:G}";
        }
        catch (Exception ex)
        {
            _model.ErrorMessage = ex.Message;
        }
    }
}
