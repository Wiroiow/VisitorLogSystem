namespace VisitorLogSystem.Models
{
    public class EmailSettings
    {
        public string Provider { get; set; } = "Smtp";
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Visitor Log System";

        public SmtpSettings Smtp { get; set; } = new SmtpSettings();
        public SendGridSettings SendGrid { get; set; } = new SendGridSettings();
        public NotificationSettings NotificationSettings { get; set; } = new NotificationSettings();
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SendGridSettings
    {
        public string ApiKey { get; set; } = string.Empty;
    }

    public class NotificationSettings
    {
        public bool SendHostNotifications { get; set; } = true;
        public bool SendVisitorConfirmations { get; set; } = true;
        public string? AdminCopyEmail { get; set; }
    }
}