using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Data;
using ProjectPRN.Models;

namespace ProjectPRN.Controllers
{
    public class SignUpController : Controller
    {
        private readonly AppDBContext _dbContext;
        public SignUpController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet] 
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = 1;
                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Login"); 
            }
            return RedirectToAction("Index",user);
        }
    }
}
