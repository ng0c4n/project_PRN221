using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectPRN.Data;
using ProjectPRN.Utils;
using System.Security.Claims;

namespace ProjectPRN.Filter
{
    public class FilterUser : ActionFilterAttribute
    {
        public bool RequireAdmin { get; set; } = false;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng họ đến trang đăng nhập
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            var userEmail = filterContext.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail == null)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }
            var dbContext = filterContext.HttpContext.RequestServices.GetRequiredService<AppDBContext>();

            var user = dbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }

            int roleId = user.Role;
            SaveUserId.AddToSession(filterContext.HttpContext, "UserId", user.ID);
            if (RequireAdmin && roleId <= 1)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
