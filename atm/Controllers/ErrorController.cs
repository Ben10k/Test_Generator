using Microsoft.AspNetCore.Mvc;

namespace atm.Controllers {
    public class ErrorController : Controller {
        // GET
        public ActionResult NotEnoughBalance() {
            ViewBag.message = "Not enough balance on your credit card";
            return View();
        }
    }
}