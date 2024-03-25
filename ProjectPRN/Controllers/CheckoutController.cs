using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Models;
using ProjectPRN.Utils;
using ProjectPRN.Data;
using ProjectPRN.Filter;
namespace ProjectPRN.Controllers
{
    [FilterUser]
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;
        public CheckoutController(PayOS payOS)
        {
            _payOS = payOS;

        }

        [HttpGet("/Checkout")]
        public async Task<IActionResult> Index()
        {

            AppDBContext _dbContext = new AppDBContext();

            // get by session
            int userID = SaveUserId.GetSessionValue<int>(HttpContext, "UserId");

            // get highest orderID of this user
            var orders = _dbContext.Order.Where(x => x.UserID == userID && x.StatusID == 1);


            var model = _dbContext.OrderDetail
                .Where(p => orders.Any(o => o.ID == p.OrderID))
                .Include(o => o.Product)
                .ToList();

            ViewBag.OrderDetail = model;
            return View();
        }
        [HttpGet("/cancel")]
        public IActionResult Cancel()
        {
            return View("cancel");
        }
        [HttpGet("/success")]
        public IActionResult Success()
        {
            AppDBContext _dbContext = new AppDBContext();
            // get by session
            int userID = SaveUserId.GetSessionValue<int>(HttpContext, "UserId");

            // get ỏderid 

            var orders = _dbContext.Order.Where(x => x.UserID == userID && x.StatusID == 1)
                .ToList();
            orders.ForEach(o => o.StatusID = 2);
            _dbContext.SaveChanges();

            return View("success");
        }
        [HttpPost("/create-payment-link")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                AppDBContext _dbContext = new AppDBContext();
                // get by session
                int userID = SaveUserId.GetSessionValue<int>(HttpContext, "UserId");
                // get list product
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                // get list 
              
                var orders = _dbContext.Order.Where(x => x.UserID == userID && x.StatusID == 1);


                var orderDetails = _dbContext.OrderDetail
                    .Where(p => orders.Any(o => o.ID == p.OrderID))
                    .Include(o => o.Product)
                    .Include(x => x.Order)
                    .ToList();
                //ItemData item = new ItemData("Mì tôm hảo hảo ly", 1, 10000);
                List<ItemData> items = new List<ItemData>();

                foreach (var item in orderDetails)
                {
                    items.Add(new ItemData(item.Product.Name,
                                item.Quantity,
                                (int)item.Product.Price));
                }

                PaymentData paymentData = new PaymentData(orderCode,
                    items.Sum(x => x.price * x.quantity),
                    "Thanh toan don hang",
                    items,
                    "https://localhost:6200/cancel",
                    "https://localhost:6200/success");

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return Redirect(createPayment.checkoutUrl);
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Redirect("https://localhost:6200/Checkout");
            }
        }
    }
}
