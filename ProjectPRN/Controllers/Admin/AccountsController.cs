using Microsoft.AspNetCore.Mvc;
using ProjectPRN.Data;
using ProjectPRN.Utils;

namespace ProjectPRN.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class AccountsController : Controller
    {
        private readonly AppDBContext _context;

        public AccountsController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View("/Views/Admin/Accounts/Account.cshtml");
        }
        [HttpGet]
        public IActionResult GetUser()
        {
            var userId = SaveUserId.GetUserID(HttpContext);
            var myAccount = _context.User.FirstOrDefault(u => u.ID == userId);
            var users = _context.User.Where(u => u.Role < myAccount.Role);
            return Ok(users);
        }
    }
}
