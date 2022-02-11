namespace Bluefish.Connections.Models;

public abstract class EmailConnectionBase : IEmailConnection
{
    protected EmailConnectionBase(string name)
    {
        Name = name;
    }

    [JsonIgnore]
    public string Name { get; private set; }

    [JsonIgnore]
    public ConnectionTypes Type => ConnectionTypes.Email;

    public abstract Task SendEmailAsync(EmailContent content);
}
