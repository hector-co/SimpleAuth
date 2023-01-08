using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.Models;

namespace SimpleAuth.Server.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string? ReturnUrl { get; set; }
        public bool Succeded { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            Succeded = result.Succeeded;

            return Page();
        }
    }
}
