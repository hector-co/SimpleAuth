using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.Models;

namespace SimpleAuth.Server.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; } = string.Empty;

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Email")]
            [EmailAddress]
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

            [Required(ErrorMessage = "Required field.")]
            [DisplayName("Code")]
            public string Code { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? code = null, string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Manage");

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };

                return Page();
            }
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
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation", new { returnUrl });
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation", new { returnUrl });
            }

            var errors = "<ul>" + string.Join("", result.Errors.Select(e => $"<li>{e}</li>")) + "</ul>";
            StatusMessage = StatusMessageModel.ErrorMessage(errors);

            return Page();
        }
    }
}
