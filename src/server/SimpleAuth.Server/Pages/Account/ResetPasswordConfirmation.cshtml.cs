using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        public string ReturnUrl { get; set; } = string.Empty;
        
        public IActionResult OnGet(string? returnUrl)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            returnUrl ??= Url.Content("~");
            ReturnUrl = returnUrl;

            return Page();
        }
    }
}
