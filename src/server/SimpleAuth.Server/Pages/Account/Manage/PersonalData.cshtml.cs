using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleAuth.Domain.Model;
using SimpleAuth.Server.ExceptionHandling;
using System.Net;

namespace SimpleAuth.Server.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<User> userManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new WebApiException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.", HttpStatusCode.NotFound, WebApiException.DataAccessError);
            }

            return Page();
        }
    }
}
