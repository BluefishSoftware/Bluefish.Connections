using System.Net;
using System.Net.Mail;

namespace Bluefish.Connections.Email;

public class SmtpConnection : EmailConnectionBase
{
    public const string DISPLAY_NAME = "SMTP";

    /// <summary>
    /// Initializes a new instance of the SmtpConnection class.
    /// </summary>
    public SmtpConnection()
        : base("SMTP")
    {
    }

    #region Settings

    public string Server { get; set; } = String.Empty;

    public string Password { get; set; } = String.Empty;

    public int Port { get; set; } = 21;

    public string Username { get; set; } = String.Empty;

    #endregion

    #region IEmailConnection

    /// <summary>
    /// Attempts to send an email.
    /// </summary>
    /// <param name="content">Email details.</param>
    public async override Task SendEmailAsync(EmailContent content)
    {
        var client = new SmtpClient(Server)
        {
            Port = Port,
            Credentials = new NetworkCredential(Username, Password),
            EnableSsl = true
        };
        var message = new MailMessage
        {
            From = new MailAddress(Username),
            Subject = content.Subject,
            Body = !string.IsNullOrWhiteSpace(content.HtmlMessage) ? content.HtmlMessage : content.PlainMessage,
            IsBodyHtml = !string.IsNullOrWhiteSpace(content.HtmlMessage)
        };
        message.To.Add(content.To);
        if (!string.IsNullOrWhiteSpace(content.Cc))
        {
            message.CC.Add(content.Cc);
        }
        if (!string.IsNullOrWhiteSpace(content.Bcc))
        {
            message.Bcc.Add(content.Bcc);
        }
        foreach (var attachment in content.Attachments)
        {
            message.Attachments.Add(new Attachment(attachment.Filename));
        }
        await client.SendMailAsync(message).ConfigureAwait(false);
    }

    #endregion
}