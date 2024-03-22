

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Models;

namespace ProjectPRN.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            AppDBContext appDBContext = new AppDBContext();
            var model = appDBContext.OrderDetail
                .Include(x => x.Order)  
                .Include(x=> x.Product)
                .ToList();
            ViewBag.OrderDetail = model;
            return View();
        }
    }
}
