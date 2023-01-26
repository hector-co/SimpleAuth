using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet(string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            return Page();
        }
    }
}
