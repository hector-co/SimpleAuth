using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Application;
using SimpleAuth.Application.Settings.Queries;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.Models;
using SimpleAuth.Server.Resources.Localizers;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Server.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly EmailResourceLocalizer _emailLocalizer;
        private readonly IMediator _mediator;

        public RegisterModel(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            EmailResourceLocalizer emailLocalizer,
            IMediator mediator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailLocalizer = emailLocalizer;
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Last name")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Email")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Password")]
            [StringLength(100, ErrorMessage = "Minimum length field.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            var setting = (await _mediator.Send(new GetServerSettingsDto())).Data;

            if (setting == null || !setting.AllowSelfRegistration)
            {
                return RedirectToPage("/Login");
            }

            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = Input.Name,
                    LastName = Input.LastName,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim { ClaimType = Claims.GivenName, ClaimValue = Input.Name },
                        new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = Input.LastName }
                    }
                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var defaultRoles = await _roleManager.Roles.Where(r => r.AssignByDefault)
                    .Select(r => r.Name).ToListAsync();
                    if (defaultRoles.Count > 0)
                    {
                        await _userManager.AddToRolesAsync(user, defaultRoles);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId, code, returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, _emailLocalizer["Confirm your email"],
                        _emailLocalizer["ConfirmEmailMessage", HtmlEncoder.Default.Encode(callbackUrl)]);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    StatusMessage = StatusMessageModel.ErrorMessage(error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
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
