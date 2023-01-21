using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OpenIddict.Abstractions;
using SimpleAuth.Application.Settings.Queries;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.Models;
using SimpleAuth.Server.Resources.Localizers;

namespace SimpleAuth.Server.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly SharedResourceLocalizer _sharedLocalizer;
        private readonly ILogger<LoginModel> _logger;
        private readonly IMediator _mediator;

        public LoginModel(SignInManager<User> signInManager, IOpenIddictApplicationManager applicationManager,
            SharedResourceLocalizer sharedLocalizer, ILogger<LoginModel> logger, IMediator mediator)
        {
            _signInManager = signInManager;
            _applicationManager = applicationManager;
            _sharedLocalizer = sharedLocalizer;
            _logger = logger;
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string? ReturnUrl { get; set; }

        public string? StatusMessage { get; set; }

        public string? ApplicationName { get; set; }

        public bool ShowRegistrationLink { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Email")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DisplayName("Remember me")]
            public bool RememberMe { get; set; }
        }

        private async Task<string?> GetApplicationName(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var parsedQuery = QueryHelpers.ParseQuery(returnUrl);
                var clientIdKey = parsedQuery.Keys.FirstOrDefault(k => Regex.IsMatch(k, @".*\?client_id$"));
                if (!string.IsNullOrEmpty(clientIdKey))
                {
                    var cliendId = parsedQuery[clientIdKey];

                    var application = await _applicationManager.FindByClientIdAsync(cliendId);

                    if (application == null)
                        return "";

                    return await _applicationManager.GetLocalizedDisplayNameAsync(application);
                }
            }

            return "";
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            var setting = (await _mediator.Send(new GetSettingDto())).Data;

            ShowRegistrationLink = setting?.AllowSelfRegistration ?? false;

            ApplicationName = await GetApplicationName(returnUrl);

            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure parsedQuery clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            StatusMessage = TempData["TempStatusMessage"]?.ToString();

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    StatusMessage = StatusMessageModel.ErrorMessage(_sharedLocalizer["This account has been locked out, please try again later."]);
                    return Page();
                }
                else
                {
                    StatusMessage = StatusMessageModel.ErrorMessage(_sharedLocalizer["Invalid login attempt."]);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
