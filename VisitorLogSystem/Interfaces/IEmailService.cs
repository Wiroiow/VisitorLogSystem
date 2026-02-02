using System;
using System.Threading.Tasks;

namespace VisitorLogSystem.Interfaces
{
    public interface IEmailService
    {
        Task SendVisitorArrivalNotificationAsync(
            string hostEmail,
            string hostName,
            string visitorName,
            string purpose,
            string roomName,
            DateTime arrivalTime);

        Task SendVisitorConfirmationEmailAsync(
            string visitorEmail,
            string visitorName,
            string roomName,
            string purpose,
            DateTime checkInTime);

        Task SendPreRegistrationConfirmationAsync(
            string visitorEmail,
            string visitorName,
            DateTime expectedDate,
            string purpose,
            string hostName);

        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody,
            string? toName = null);

        Task<bool> TestEmailConfigurationAsync(string testEmail);
    }
}