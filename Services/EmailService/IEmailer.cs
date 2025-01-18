using Microsoft.AspNetCore.Identity;

namespace YawShop.Services.EmailService
{
    /// <summary>
    /// Interface for sending emails.
    /// </summary>
    public interface IEmailer : IEmailSender<IdentityUser>
    {
        /// <summary>
        /// Sends email async.
        /// </summary>
        /// <param name="EmailMessage"></param>
        /// <returns></returns>
        public Task SendMailAsync(EmailMessage EmailMessage);

    }
}
