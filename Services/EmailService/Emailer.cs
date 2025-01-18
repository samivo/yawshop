
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace YawShop.Services.EmailService
{
    /// <summary>
    /// Sends a email using smtp client
    /// </summary>
    public class Emailer : IEmailer
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="Emailer"/> class with SMTP configuration.
        /// </summary>
        public Emailer(IOptions<SmtpSettings> smtpSettings, ILogger<Emailer> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// <Sends cref="EmailMessage"/> async. BCC is used if multipe recipient.
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public async Task SendMailAsync(EmailMessage emailMessage)
        {
            try
            {
                if (emailMessage.To.Count <= 0)
                {
                    throw new Exception("No email recipients found.");
                }

                using var message = new MimeMessage();

                message.Subject = emailMessage.Subject;
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));

                //Set first recipient To
                message.To.Add(new MailboxAddress(emailMessage.To.ElementAt(0), emailMessage.To.ElementAt(0)));
                ((List<string>)emailMessage.To).RemoveAt(0);


                //Use Bcc when multiple receivers
                if (emailMessage.To.Count > 0)
                {
                    foreach (var receiver in emailMessage.To)
                    {
                        message.Bcc.Add(new MailboxAddress(receiver, receiver));
                    }
                }

                message.Body = emailMessage.Body;

                using var client = new SmtpClient();

                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email: {message}", ex.Message);
                throw;
            }

            return;
        }

        Task IEmailSender<IdentityUser>.SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
        {
            throw new NotImplementedException();
        }

        Task IEmailer.SendMailAsync(EmailMessage EmailMessage)
        {
            throw new NotImplementedException();
        }

        Task IEmailSender<IdentityUser>.SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        Task IEmailSender<IdentityUser>.SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Testing emailer class
    /// </summary>
    public class TestEmailer : IEmailer
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Emailer"/> class with SMTP configuration.
        /// </summary>
        public TestEmailer(IOptions<SmtpSettings> smtpSettings, ILogger<TestEmailer> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
        {
            Console.WriteLine("Mail from emailer, send confirmation link to " + user.Email);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Test: send asyncs. Same as real one, but wont actually send the email.
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public async Task SendMailAsync(EmailMessage emailMessage)
        {
            try
            {
                if (emailMessage.To.Count <= 0)
                {
                    _logger.LogError("No recipients in EmailMessage?");
                    throw new Exception("No email recipients found.");
                }

                var message = new MimeMessage();

                message.Subject = emailMessage.Subject;
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));

                _logger.LogInformation("From: {name}, {email}", _smtpSettings.SenderName, _smtpSettings.SenderEmail);

                //Set first recipient To
                message.To.Add(new MailboxAddress(emailMessage.To.ElementAt(0), emailMessage.To.ElementAt(0)));

                _logger.LogInformation("To: {name}, {email}", emailMessage.To.ElementAt(0), emailMessage.To.ElementAt(0));

                ((List<string>)emailMessage.To).RemoveAt(0);


                //Use Bcc when multiple receivers
                if (emailMessage.To.Count > 0)
                {
                    foreach (var receiver in emailMessage.To)
                    {
                        message.Bcc.Add(new MailboxAddress(receiver, receiver));
                        _logger.LogDebug("BCC: {name}, {email}", receiver, receiver);
                    }
                }

                message.Body = emailMessage.Body;

                //Make sure there is no email to send
                message.Dispose();
                message = null;

                using var client = new SmtpClient();

                //Test the connection to smtp server
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);

                await client.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                _logger.LogDebug("Exception catch while sending email: {message}", ex.Message);
                throw;
            }

            return;
        }

        public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
        {
            Console.WriteLine("Mail from emailer, send passwordreset code to " + user.Email);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
        {
            Console.WriteLine("Mail from emailer, send passwordreset link to " + user.Email);
            return Task.CompletedTask;
        }
    }
}
