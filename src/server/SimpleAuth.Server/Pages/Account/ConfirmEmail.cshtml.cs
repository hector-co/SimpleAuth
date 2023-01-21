using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.ExceptionHandling;
using SimpleAuth.Server.Models;
using SimpleAuth.Server.Resources.Localizers;

namespace SimpleAuth.Server.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SharedResourceLocalizer _sharedLocalizer;

        public ConfirmEmailModel(UserManager<User> userManager, SharedResourceLocalizer sharedLocalizer)
        {
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer;
        }

        public string? ReturnUrl { get; set; }
        public bool Succeded { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string? returnUrl)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (userId == null || code == null)
            {
                return RedirectToPage("/Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new WebApiException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.", HttpStatusCode.NotFound, WebApiException.DataAccessError);
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            Succeded = result.Succeeded;

            return Page();
        }
    }
}
