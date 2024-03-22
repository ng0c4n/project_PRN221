using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Models;

namespace ProjectPRN.Controllers.Admin;

[Route("Admin/[controller]/[action]")]
public class DashboardsController : Controller
{
    private readonly AppDBContext _context;

    public DashboardsController(AppDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var productToday = _context.Product.Count(p => p.CreateAt >= DateTime.Today);
        var productBeforeToday = _context.Product.Count(p => p.CreateAt < DateTime.Today);

        var orderToday = _context.Order.Where(p => p.CreatedDate >= DateTime.Today)
            .Include(p => p.OrderDetails)
            .ToList();
        var orderYesterday= _context.Order.Where(p => p.CreatedDate >= DateTime.Today.AddDays(-1) && p.CreatedDate < DateTime.Today)
            .Include(p => p.OrderDetails)
            .ToList();

        decimal ProfitToday = 0;
        foreach (var order in orderToday)
        {
            if (order!= null)
            {
                foreach (var orderDetail in order.OrderDetails) 
                {
                    var product = _context.Product.Find(orderDetail.ProductID);
                    ProfitToday += product != null ? product.Price : 0;
                }

            }
        }

        decimal ProfitYesterday = 0;
        foreach (var order in orderYesterday)
        {
            if (order != null)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = _context.Product.Find(orderDetail.ProductID);
                    ProfitYesterday += product != null ? product.Price : 0;
                }

            }
        }
        ViewBag.ProductToday = productToday;
        ViewBag.ProductBeforeToday = productBeforeToday;
        ViewBag.ProfitToday = ProfitToday;
        ViewBag.ProfitYesterday = ProfitYesterday;
        ViewBag.OrderToday = orderToday.Count();
        ViewBag.OrderYesterday = orderYesterday.Count();

        return View("/Views/Admin/Dashboards/Index.cshtml");
    }
}
