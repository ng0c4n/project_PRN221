using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
namespace ProjectPRN.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;

        public CheckoutController(PayOS payOS)
        {
            _payOS = payOS;

        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View("index");
        }
        [HttpGet("/cancel")]
        public IActionResult Cancel()
        {
            return View("cancel");
        }
        [HttpGet("/success")]
        public IActionResult Success()
        {
            return View("success");
        }
        [HttpPost("/create-payment-link")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData("Mì tôm hảo hảo ly", 1, 10000);
                List<ItemData> items = new List<ItemData>();
                items.Add(item);
                PaymentData paymentData = new PaymentData(orderCode, item.price, "Thanh toan don hang", items, "https://localhost:7102/cancel", "https://localhost:7102/success");

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
