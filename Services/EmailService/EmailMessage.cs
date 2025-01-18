using MimeKit;

namespace YawShop.Services.EmailService
{
    /// <summary>
    /// Holds basic information about email message
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// The subject of the email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body of the email in plain text
        /// </summary>
        public TextPart Body { get; set; }

        /// <summary>
        /// A collection of recipient email addresses. Bcc will be used if multiple receivers.
        /// </summary>
        public ICollection<string> To { get; set; }

        /// <summary>
        ///Initializes a new instance of the <see cref="EmailMessage"/> class with empty fields.
        /// </summary>
        public EmailMessage()
        {
            Subject = "";
            Body = new TextPart();
            To = new List<string>();

        }

        /// <summary>
        ///Initializes a new instance of the <see cref="EmailMessage"/> class with specified parameters.
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="To"></param>
        public EmailMessage(string Subject, TextPart Body, ICollection<string> To)
        {
            this.Subject = Subject;
            this.Body = Body;
            this.To = To;
        }
    }
}
