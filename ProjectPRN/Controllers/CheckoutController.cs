using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Microsoft.EntityFrameworkCore;
using ProjectPRN.Models;
namespace ProjectPRN.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;

        public CheckoutController(PayOS payOS)
        {
            _payOS = payOS;

        }

        [HttpGet("/Checkout")]
        public IActionResult Index()
        {

            AppDBContext _dbContext = new AppDBContext();

            // get by session
            int userID = 2;

            // get highest orderID of this user
            int orderID = _dbContext.Order.Where(x => x.UserID == userID && x.StatusID == 1).Max(x => x.ID);

            var model = _dbContext.OrderDetail
                .Where(x => x.OrderID == orderID)
                .Include(x => x.Order)
                .Include(x => x.Product)
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
            int userID = 2;
            // get ỏderid 
            var orderID = _dbContext.Order
                .Where(x => x.UserID == userID).Max(a => a.ID);
            // get order
            var order = _dbContext.Order
                .SingleOrDefault(x => x.ID == orderID);

            // updating
            order.StatusID = 2;
            _dbContext.Attach(order).State = EntityState.Modified;
            _dbContext.SaveChanges();

            // Creating
            _dbContext = new AppDBContext();
            _dbContext.Add(new Order()
            {
                StatusID = 1,
                UserID = userID,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            });
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
                int userID = 2;
                // get list product
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                // get list 
                int orderID = _dbContext.Order.Where(x => x.UserID == userID && x.StatusID == 1).Max(x => x.ID);

                var orderDetails = _dbContext.OrderDetail
                                    .Where(x => x.OrderID == orderID)
                                    .Include(x => x.Order)
                                    .Include(x => x.Product)
                                    .ToList();

                //ItemData item = new ItemData("Mì tôm hảo hảo ly", 1, 10000);
                List<ItemData> items = new List<ItemData>();
                
                foreach(var item in orderDetails)
                {
                    items.Add(new ItemData(item.Product.Name, 
                                item.Quantity,
                                (int) item.Product.Price));
                }
                
                PaymentData paymentData = new PaymentData(orderCode, 
                    items.Sum(x=> x.price * x.quantity), 
                    "Thanh toan don hang", 
                    items, 
                    "https://localhost:7102/cancel", 
                    "https://localhost:7102/success");

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return Redirect(createPayment.checkoutUrl);
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Redirect("https://localhost:7102/");
            }
        }
    }
}
