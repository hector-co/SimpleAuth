using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            return Page();
        }
    }
}
