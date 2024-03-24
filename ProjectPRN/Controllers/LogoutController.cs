using Microsoft.AspNetCore.Mvc;

namespace ProjectPRN.Controllers
{
    public class LogoutController : Controller
    {
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}
