using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bluefish.Connections.Email;

/// <summary>
/// The SendGridConnection class implements a connection that sends email messages.
/// </summary>
public class SendGridConnection : EmailConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the SendGridConnection class.
    /// </summary>
    public SendGridConnection()
        : base("SendGrid")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the API key to use to access the SendGrid API.
    /// </summary>
    public string ApiKey { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the email address of the sender.
    /// </summary>
    public string From { get; set; } = String.Empty;

    #endregion

    #region IEmailConnection

    /// <summary>
    /// Attempts to send an email.
    /// </summary>
    /// <param name="content">Email details.</param>
    public async override Task SendEmailAsync(EmailContent content)
    {
        var client = new SendGridClient(ApiKey);
        var fromAddress = new EmailAddress(From);
        var toAddress = new EmailAddress(content.To);
        var messageAttachments = new List<Attachment>();
        foreach (var attachment in content.Attachments)
        {
            messageAttachments.Add(new Attachment
            {
                Filename = attachment.Filename,
                Type = attachment.Type,
                Content = Convert.ToBase64String(attachment.Content)
            });
        }

        var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, content.Subject, content.PlainMessage, content.HtmlMessage);
        if (!string.IsNullOrWhiteSpace(content.Cc))
        {
            msg.AddCc(new EmailAddress(content.Cc));
        }
        if (!string.IsNullOrWhiteSpace(content.Bcc))
        {
            msg.AddBcc(new EmailAddress(content.Bcc));
        }
        msg.AddAttachments(messageAttachments);
        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
    }

    #endregion
}