using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Utils;
using ProjectPRN.Data;

namespace ProjectPRN.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDBContext _dbContext;
        public LoginController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _dbContext.User.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                SaveUserId.AddToSession(HttpContext,"UserId",user.ID);
                if (user.Role > 1)
                {
                    return Redirect("/admin/dashboards/index");
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Index");
            }
        }

        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }).ToList();
            var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                Save(email, name);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        private void Save(string email, string name)
        {
            var user = _dbContext.User.FirstOrDefault(u => u.Email == email);
            if (user == null) 
            {
                user = new User
                {
                    Email = email,
                    Name = name,
                    Password = email,
                    Role = 1,
                };
                _dbContext.User.Add(user);
            }
            _dbContext.SaveChanges(); 
        }
    }
}
