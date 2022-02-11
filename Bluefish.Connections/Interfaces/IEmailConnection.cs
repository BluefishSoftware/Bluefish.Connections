namespace Bluefish.Connections.Interfaces;

/// <summary>
/// The IEmailConnection interface defines the required properties and method for an Email Connection.
/// </summary>
public interface IEmailConnection : IConnection
{
    /// <summary>
    /// Attempts to send an email.
    /// </summary>
    /// <param name="content">Email details.</param>
    public abstract Task SendEmailAsync(EmailContent content);
}
