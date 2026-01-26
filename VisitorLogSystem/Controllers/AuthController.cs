using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLogSystem.DTOs;
using VisitorLogSystem.Interfaces;
using VisitorLogSystem.ViewModels;

namespace VisitorLogSystem.Controllers
{
    
    /// Authentication Controller - Handles login/logout requests
   
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        /// Shows the login page
        
        /// If user is already logged in, redirect to Dashboard
        
        [HttpGet]
        [AllowAnonymous]  
        public IActionResult Login(string? returnUrl = null)
        {
            // Check if user is already logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                
                return RedirectToAction("Index", "Dashboard");
            }

            // Store return URL for redirect after login
            ViewData["ReturnUrl"] = returnUrl;

            // Show login form
            return View(new LoginViewModel());
        }

       
        /// Process login form submission
    
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // Store return URL for view
            ViewData["ReturnUrl"] = returnUrl;

            // STEP 1: Validate form data
            if (!ModelState.IsValid)
            {
                
                return View(model);
            }

            // STEP 2: Convert ViewModel to DTO
            var loginDto = new LoginDto
            {
                Username = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            // STEP 3: Validate credentials using AuthService
            var user = await _authService.ValidateLoginAsync(loginDto);

            if (user == null)
            {
               
                model.ErrorMessage = "Invalid username or password.";
                return View(model);
            }

            // STEP 4: Create claims (digital ID card)
            var claimsPrincipal = _authService.CreateClaimsPrincipal(user);

            // STEP 5: Create authentication properties
            var authProperties = new AuthenticationProperties
            {
                // Set cookie expiration
                IsPersistent = model.RememberMe, 

             
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(1)
            };

            // STEP 6: Sign in the user
            // This creates the authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                claimsPrincipal, // User's claims (ID card)
                authProperties   
            );

            // STEP 7: Redirect to appropriate page
            // If there's a return URL (user tried to access protected page), go there
            // Otherwise, go to dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

       
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            
            // This deletes the authentication cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            
            return RedirectToAction(nameof(Login));
        }

       
        
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}