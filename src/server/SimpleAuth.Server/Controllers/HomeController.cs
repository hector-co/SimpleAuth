using Microsoft.AspNetCore.Mvc;

namespace SimpleAuth.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return LocalRedirect("/Account/Manage");
            else
                return LocalRedirect("/Account/Login");
        }
    }
}
