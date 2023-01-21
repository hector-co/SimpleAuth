using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SimpleAuth.Application;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using SimpleAuth.Domain.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.ComponentModel;
using SimpleAuth.Server.Resources.Localizers;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SimpleAuth.Application.Settings.Queries;
using SimpleAuth.Server.Models;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly SharedResourceLocalizer _sharedLocalizer;
        private readonly EmailResourceLocalizer _emailLocalizer;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IMediator _mediator;

        public ExternalLoginModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUserStore<User> userStore,
            SharedResourceLocalizer sharedLocalizer,
            EmailResourceLocalizer emailLocalizer,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IMediator mediator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _sharedLocalizer = sharedLocalizer;
            _emailLocalizer = emailLocalizer;
            _logger = logger;
            _emailSender = emailSender;
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Email")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                TempData["TempStatusMessage"] = _sharedLocalizer["Error from external provider", remoteError];
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["TempStatusMessage"] = _sharedLocalizer["Error loading external login information."];
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                TempData["TempStatusMessage"] = StatusMessageModel.ErrorMessage(_sharedLocalizer["This account has been locked out, please try again later."]);
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            else
            {
                var setting = (await _mediator.Send(new GetSettingDto())).Data!;
                if (!setting.AllowSelfRegistration)
                {
                    TempData["TempStatusMessage"] = StatusMessageModel.ErrorMessage(_sharedLocalizer["User not found."]);
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                }

                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["TemStatusMessage"] = _sharedLocalizer["Error loading external login information during confirmation."];
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var nameClaim = info.Principal.Claims.First(c => c.Type == ClaimTypes.GivenName);
                var lastNameClaim = info.Principal.Claims.First(c => c.Type == ClaimTypes.Surname);

                var user = new User
                {
                    Name = nameClaim.Value,
                    LastName = lastNameClaim.Value,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim { ClaimType = Claims.GivenName, ClaimValue = nameClaim.Value },
                        new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = lastNameClaim.Value }
                    }
                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var defaultRoles = await _roleManager.Roles.Where(r => r.AssignByDefault)
                    .Select(r => r.Name).ToListAsync();
                    if (defaultRoles.Count > 0)
                    {
                        await _userManager.AddToRolesAsync(user, defaultRoles);
                    }

                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId, code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, _emailLocalizer["Confirm your email"],
                            _emailLocalizer["ConfirmEmailMessage", HtmlEncoder.Default.Encode(callbackUrl)]);

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
