using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.ExceptionHandling;
using SimpleAuth.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Server.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } = string.Empty;
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Name,
                LastName = user.LastName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new WebApiException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.", HttpStatusCode.NotFound, WebApiException.DataAccessError);
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new WebApiException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.", HttpStatusCode.NotFound, WebApiException.DataAccessError);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _userManager.ReplaceClaimAsync(user, new Claim(Claims.GivenName, user.Name), new Claim(Claims.GivenName, Input.Name));
            await _userManager.ReplaceClaimAsync(user, new Claim(Claims.FamilyName, user.LastName), new Claim(Claims.FamilyName, Input.LastName));

            user.Name = Input.Name;
            user.LastName = Input.LastName;
            await _userManager.UpdateAsync(user);

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber ?? string.Empty);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = StatusMessageModel.ErrorMessage("Unexpected error when trying to set phone number.");
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = StatusMessageModel.SuccessMessage("Your profile has been updated");
            return RedirectToPage();
        }
    }
}
