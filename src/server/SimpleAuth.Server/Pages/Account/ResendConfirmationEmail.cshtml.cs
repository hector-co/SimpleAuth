using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SimpleAuth.Application;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.Models;
using SimpleAuth.Server.Resources.Localizers;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class ResendConfirmationEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SharedResourceLocalizer _sharedLocalizer;
        private readonly EmailResourceLocalizer _emailLocalizer;

        public ResendConfirmationEmailModel(UserManager<User> userManager, IEmailSender emailSender, SharedResourceLocalizer sharedLocalizer, 
            EmailResourceLocalizer emailLocalizer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _sharedLocalizer = sharedLocalizer;
            _emailLocalizer = emailLocalizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Email")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? returnUrl)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                StatusMessage = StatusMessageModel.SuccessMessage(_sharedLocalizer["Verification email sent. Please check your email."]);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, code, returnUrl },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.Email,
                _emailLocalizer["Confirm your email"],
                _emailLocalizer["ConfirmEmailMessage", HtmlEncoder.Default.Encode(callbackUrl)]);

            StatusMessage = StatusMessageModel.SuccessMessage(_sharedLocalizer["Verification email sent. Please check your email."]);

            return Page();
        }
    }
}
