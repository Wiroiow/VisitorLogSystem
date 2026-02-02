using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendVisitorArrivalNotificationAsync(
            string hostEmail,
            string hostName,
            string visitorName,
            string purpose,
            string roomName,
            DateTime arrivalTime)
        {
            if (!_emailSettings.NotificationSettings.SendHostNotifications)
                return;

            var subject = $"Visitor Arrival: {visitorName}";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 20px 0; border-radius: 8px; border-left: 4px solid #667eea; }}
        .info-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #667eea; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔔 Visitor Has Arrived</h1>
        </div>
        <div class='content'>
            <p>Hi <strong>{hostName}</strong>,</p>
            <p>Your visitor has checked in and is on their way to meet you.</p>
            
            <div class='info-box'>
                <div class='info-row'>
                    <span class='label'>Visitor Name:</span> {visitorName}
                </div>
                <div class='info-row'>
                    <span class='label'>Purpose:</span> {purpose}
                </div>
                <div class='info-row'>
                    <span class='label'>Room:</span> {roomName}
                </div>
                <div class='info-row'>
                    <span class='label'>Arrival Time:</span> {arrivalTime:h:mm tt}
                </div>
            </div>

            <p>Please proceed to <strong>{roomName}</strong> to meet your visitor.</p>
            
            <div class='footer'>
                <p>This is an automated notification from Visitor Log System</p>
                <p>© {DateTime.Now.Year} Visitor Log System. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(hostEmail, subject, htmlBody, hostName);
        }

        public async Task SendVisitorConfirmationEmailAsync(
            string visitorEmail,
            string visitorName,
            string roomName,
            string purpose,
            DateTime checkInTime)
        {
            if (!_emailSettings.NotificationSettings.SendVisitorConfirmations)
                return;

            var subject = "Check-In Confirmation - Visitor Log System";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .success-icon {{ font-size: 60px; margin-bottom: 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 20px 0; border-radius: 8px; }}
        .info-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #059669; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='success-icon'>✅</div>
            <h1>Check-In Successful!</h1>
        </div>
        <div class='content'>
            <p>Hi <strong>{visitorName}</strong>,</p>
            <p>You have successfully checked in. Here are your visit details:</p>
            
            <div class='info-box'>
                <div class='info-row'>
                    <span class='label'>Check-In Time:</span> {checkInTime:MMMM dd, yyyy h:mm tt}
                </div>
                <div class='info-row'>
                    <span class='label'>Room:</span> {roomName}
                </div>
                <div class='info-row'>
                    <span class='label'>Purpose:</span> {purpose}
                </div>
            </div>

            <p>Please remember to check out when you leave.</p>
            <p>Thank you for visiting!</p>
            
            <div class='footer'>
                <p>This is an automated confirmation from Visitor Log System</p>
                <p>© {DateTime.Now.Year} Visitor Log System. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(visitorEmail, subject, htmlBody, visitorName);
        }

        public async Task SendPreRegistrationConfirmationAsync(
            string visitorEmail,
            string visitorName,
            DateTime expectedDate,
            string purpose,
            string hostName)
        {
            var subject = "Pre-Registration Confirmed - Visitor Log System";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 20px 0; border-radius: 8px; }}
        .info-row {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #2563eb; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
        .reminder {{ background: #fef3c7; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #f59e0b; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📅 Pre-Registration Confirmed</h1>
        </div>
        <div class='content'>
            <p>Hi <strong>{visitorName}</strong>,</p>
            <p>Your visit has been pre-registered. Here are the details:</p>
            
            <div class='info-box'>
                <div class='info-row'>
                    <span class='label'>Expected Visit Date:</span> {expectedDate:MMMM dd, yyyy}
                </div>
                <div class='info-row'>
                    <span class='label'>Purpose:</span> {purpose}
                </div>
                <div class='info-row'>
                    <span class='label'>Host:</span> {hostName}
                </div>
            </div>

            <div class='reminder'>
                <strong>💡 When you arrive:</strong><br>
                Simply enter your name at the check-in kiosk, and we'll have your information ready!
            </div>

            <p>We look forward to seeing you!</p>
            
            <div class='footer'>
                <p>This is an automated confirmation from Visitor Log System</p>
                <p>© {DateTime.Now.Year} Visitor Log System. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(visitorEmail, subject, htmlBody, visitorName);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string? toName = null)
        {
            try
            {
                await SendViaSmtpAsync(toEmail, subject, htmlBody, toName);

                if (!string.IsNullOrWhiteSpace(_emailSettings.NotificationSettings.AdminCopyEmail))
                {
                    await SendViaSmtpAsync(
                        _emailSettings.NotificationSettings.AdminCopyEmail,
                        $"[COPY] {subject}",
                        htmlBody
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email send failed: {ex.Message}");
                throw;
            }
        }

        private async Task SendViaSmtpAsync(string toEmail, string subject, string htmlBody, string? toName = null)
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
            message.To.Add(new MailAddress(toEmail, toName ?? toEmail));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_emailSettings.Smtp.Host, _emailSettings.Smtp.Port);
            smtpClient.EnableSsl = _emailSettings.Smtp.EnableSsl;
            smtpClient.Credentials = new NetworkCredential(
                _emailSettings.Smtp.Username,
                _emailSettings.Smtp.Password
            );

            await smtpClient.SendMailAsync(message);
        }

        public async Task<bool> TestEmailConfigurationAsync(string testEmail)
        {
            try
            {
                var subject = "Email Configuration Test - Visitor Log System";
                var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2 style='color: #10b981;'>✅ Email Configuration Test Successful!</h2>
    <p>If you're reading this, your email settings are working correctly.</p>
    <p><strong>Timestamp:</strong> {DateTime.Now:F}</p>
    <hr>
    <p style='color: #666; font-size: 12px;'>Visitor Log System</p>
</body>
</html>";

                await SendEmailAsync(testEmail, subject, body);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}