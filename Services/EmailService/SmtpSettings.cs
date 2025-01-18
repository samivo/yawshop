namespace YawShop.Services.EmailService
{
    /// <summary>
    /// Smpt settings for Emailer. See appsettings.json
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Host name to smtp server.
        /// <example>mail.mydomain.com</example>
        /// </summary>
        public required string Host { get; set; }

        /// <summary>
        /// Server port number
        /// </summary>
        public required int Port { get; set; }

        /// <summary>
        /// username to account
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Name that recipient sees is message
        /// </summary>
        public required string SenderName { get; set; }

        /// <summary>
        /// Email address that recipient sees / where the mail send from.
        /// </summary>
        public required string SenderEmail { get; set; }
    }
}
