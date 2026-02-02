using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisitorLogSystem.Interfaces;

namespace VisitorLogSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmailTestController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTest(string testEmail)
        {
            if (string.IsNullOrWhiteSpace(testEmail))
            {
                TempData["ErrorMessage"] = "Please enter an email address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var success = await _emailService.TestEmailConfigurationAsync(testEmail);

                if (success)
                {
                    TempData["SuccessMessage"] = $"✅ Test email sent successfully to {testEmail}! Check your inbox (and spam folder).";
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ Email test failed. Check your configuration.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Email test failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}