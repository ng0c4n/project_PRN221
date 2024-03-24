using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Data;
using ProjectPRN.Utils;

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
        return View("/Views/Admin/Dashboards/Index.cshtml");
    }

    [HttpGet]
    public IActionResult DashboardData()
    {
        var productToday = _context.Product.Count(p => p.CreateAt >= DateTime.Today);
        var productBeforeToday = _context.Product.Count(p => p.CreateAt < DateTime.Today);

        var orderToday = _context.Order.Where(p => p.CreatedDate >= DateTime.Today)
            .Include(p => p.OrderDetails)
            .ToList();
        var orderYesterday = _context.Order.Where(p => p.CreatedDate >= DateTime.Today.AddDays(-1) && p.CreatedDate < DateTime.Today)
            .Include(p => p.OrderDetails)
            .ToList();

        Dictionary<int, int> orderStatics = new Dictionary<int, int>();
        Dictionary<int, decimal> orderStaticsPrice = new Dictionary<int, decimal>();
        foreach (var item in _context.Category.ToList())
        {
            orderStatics.Add(item.ID, 0);
            orderStaticsPrice.Add(item.ID, 0);
        }
        decimal profitToday = 0;
        foreach (var order in orderToday)
        {
            if (order != null)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = _context.Product.Find(orderDetail.ProductID);
                    profitToday += product != null ? orderDetail.Quantity * product.Price : 0;
                    orderStatics[product.CategoryID] += orderDetail.Quantity;
                    orderStaticsPrice[product.CategoryID] += orderDetail.Quantity * product.Price;

                }

            }
        }

        decimal profitYesterday = 0;
        foreach (var order in orderYesterday)
        {
            if (order != null)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = _context.Product.Find(orderDetail.ProductID);
                    profitYesterday += product != null ? product.Price : 0;
                }

            }
        }
        //-------------Order Static

        var totalOrderDetails = 0;
        OrderStaticsObject orderStaticsObjects = new OrderStaticsObject();
        foreach (var orderStatic in orderStatics)
        {
            var categoryName = _context.Category.Find(orderStatic.Key).Name;
            orderStaticsObjects.Name.Add(categoryName);
            orderStaticsObjects.Value.Add(orderStatic.Value);
            orderStaticsObjects.Price.Add(orderStaticsPrice[orderStatic.Key]);
            totalOrderDetails += orderStatic.Value;

        }

        decimal totalIncomeInYear = 0;
        var ordersInYear = _context.Order
            .Where(o => o.CreatedDate >= DateTime.Now.AddYears(-1))
            .OrderBy(o => o.CreatedDate)
            .ToList();

        Dictionary<string, decimal> ordersInMonth = new Dictionary<string, decimal>();
        foreach (var order in ordersInYear)
        {
            int month = order.CreatedDate.Month;
            int year = order.CreatedDate.Year;
            string key = ""+month + '-' + year;
            if (!ordersInMonth.ContainsKey(key))
            {
                ordersInMonth.Add(key, 0);
            }
            var orderDetails = _context.OrderDetail
            .Where(od => od.OrderID == order.ID)
            .Include(o => o.Product)
            .ToList();

            foreach (var orderDetail in orderDetails)
            {
                ordersInMonth[key] += orderDetail.Quantity * orderDetail.Product.Price;
                totalIncomeInYear += orderDetail.Quantity * orderDetail.Product.Price;
            }
        }
        var userName = _context.User.FirstOrDefault(p => p.ID == SaveUserId.GetUserID(HttpContext)).Name;
        //-------------
        var result = new
        {
            ProductToday = productToday,
            ProductBeforeToday = productBeforeToday,
            ProfitToday = profitToday,
            ProfitYesterday = profitYesterday,
            OrderToday = orderToday.Count(),
            OrderYesterday = orderYesterday.Count(),
            IncreasePercentProfit = profitYesterday != 0 ? profitToday / profitYesterday * 100 : 100,
            IncreasePercentProduct = productBeforeToday != 0 ? productToday / productBeforeToday * 100 : 100,
            OrderStaticsObjects = orderStaticsObjects,
            TotalOrderDetails = totalOrderDetails,
            TotalIncomeInYear = totalIncomeInYear,
            OrdersInMonth = ordersInMonth.ToList(),
            UserName = userName,
        };

        return Ok(result);
    }
}


class OrderStaticsObject
{
    public List<string> Name { get; set; } = new List<string>();
    public List<int> Value { get; set; } = new List<int>();
    public List<decimal> Price { get; set; } = new List<decimal>();
}