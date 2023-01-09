using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreIdentityWithMongoDB.Controllers
{
    public class SecuredController : Controller
    {
        [Authorize(Roles ="Admin")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
