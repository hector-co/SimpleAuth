using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleAuth.Server.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        public string ReturnUrl { get; set; } = string.Empty;
        
        public void OnGet(string? returnUrl)
        {
            returnUrl ??= Url.Content("~");
            ReturnUrl = returnUrl;
        }
    }
}
