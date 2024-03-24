using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Login"); 
            }
            return View(user);
        }
    }
}
