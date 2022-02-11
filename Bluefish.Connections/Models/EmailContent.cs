namespace Bluefish.Connections.Models;

public class EmailContent
{
    public IEnumerable<EmailAttachment> Attachments { get; set; } = Array.Empty<EmailAttachment>();
    public string Bcc { get; set; } = string.Empty;
    public string Cc { get; set; } = string.Empty;
    public string HtmlMessage { get; set; } = string.Empty;
    public string PlainMessage { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}
