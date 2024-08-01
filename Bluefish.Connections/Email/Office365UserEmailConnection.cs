using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Bluefish.Connections.Email;

public class Office365UserEmailConnection : EmailConnectionBase
{
    /// <summary>
    /// Initializes a new instance of the Office365UserEmailConnection class.
    /// </summary>
    public Office365UserEmailConnection()
        : base("Office365User")
    {
    }

    #region Settings

    /// <summary>
    /// Gets or sets the Tenant Identifier key to use to access Azure AD.
    /// </summary>
    public string TenantId { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the Client Identifier key to use to access Azure AD.
    /// </summary>
    public string ClientId { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the Client Secret key to use to access Azure AD.
    /// </summary>
    public string ClientSecret { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the User Identifier key to use to access Azure AD.
    /// </summary>
    public string ObjectId { get; set; } = String.Empty;

    #endregion

    #region IEmailConnection

    /// <summary>
    /// Attempts to send an email.
    /// </summary>
    /// <param name="content">Email details.</param>
    public async override Task SendEmailAsync(EmailContent content)
    {
        // Define your credentials based on the created app and user details.
        // Specify the options. In most cases we're running the Azure Public Cloud.
        var credentials = new ClientSecretCredential(
            TenantId,
            ClientId,
            ClientSecret,
            new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

        // Define our new Microsoft Graph client.
        // Use the credentials we specified above.
        var graphServiceClient = new GraphServiceClient(credentials);

        var message = new Message
        {
            Subject = content.Subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = content.HtmlMessage
            },
            ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = content.To
                        }
                    }
                }
        };

        if (!string.IsNullOrWhiteSpace(content.Cc))
        {
            message.CcRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = content.Cc
                        }
                    }
                };
        }
        if (!string.IsNullOrWhiteSpace(content.Bcc))
        {
            message.BccRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = content.Bcc
                        }
                    }
                };
        }

        // Send mail as the given user.

        // Please note that since Graph API v. 5.0 (march 2023), Microsoft introduced an annoying breaking change,
        // not really well documented. Now, SendMail cannot be used anymore as a method of a User. You will get a
        // CS1955 Non-invocable member cannot be used like a method compiler error if you try to do that.

        Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody requestbody = new()
        {
            Message = message,
            SaveToSentItems = false // or true, as you want
        };

        await graphServiceClient.Users[ObjectId]
            .SendMail
            .PostAsync(requestbody);
    }

    #endregion
}
