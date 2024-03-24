using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Data;
using ProjectPRN.Utils;
using SignalRAssignment;

namespace ProjectPRN.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class AccountsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly SignalHub _signalHub;

        public AccountsController(AppDBContext context, SignalHub signalHub)
        {
            _context = context;
            _signalHub = signalHub;
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
            var users = _context.User.Where(u => u.Role < myAccount.Role)
                .Include(p => p.UserRole);
            var roleList = _context.UserRole.Where(r => r.ID < myAccount.Role);
            var result = new
            {
                Users = users,
                RoleList = roleList,
            };
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SetRole(int? UserId, int? RoleID)
        {
            if (UserId == null || RoleID == null)
            {
                return BadRequest();
            }
            var user = _context.User.FirstOrDefault(u => u.ID == UserId);
            if (user == null)
            {
                return BadRequest();
            }
            var role = _context.UserRole.FirstOrDefault(r => r.ID == RoleID);
            if (role == null)
            {
                return BadRequest();
            }
            user.Role = RoleID ?? 1;
            _context.Update(user);
            _context.SaveChanges();
            await _signalHub.Clients.All.SendAsync("LoadUserAccount");
            return Ok();
        }
    }
}
