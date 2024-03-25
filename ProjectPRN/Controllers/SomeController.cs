using Microsoft.AspNetCore.Mvc;
using ProjectPRN.Filter;

namespace ProjectPRN.Controllers
{
    public class SomeController : Controller
    {
        [FilterUser]
        public ActionResult SomeAction()
        {
            return View();
        }
    }
}
