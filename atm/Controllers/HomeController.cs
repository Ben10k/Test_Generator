using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using atm.Models;
using Microsoft.AspNetCore.Http;

namespace atm.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }


        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password) {
            return RedirectToAction("Menu");
        }

        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string email, string password) {
            return RedirectToAction("Login");
        }


        public ActionResult Menu() {
            return View();
        }

        public ActionResult Deposit() {
            return View();
        }

        public ActionResult Withdraw() {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(int amount) {
            Singleton.Instance.balance += amount;
            return RedirectToAction("Menu");
        }

        [HttpPost]
        public ActionResult Withdraw(int amount) {
            if (amount > Singleton.Instance.balance) {
                return RedirectToAction("NotEnoughBalance", "Error");
            }

            Singleton.Instance.balance -= amount;
            return RedirectToAction("Menu");
        }

        public ActionResult Check() {
            ViewBag.Balance = Singleton.Instance.balance;
            return View();
        }

        public ActionResult Logout() {
            return RedirectToAction("Index");
        }
    }
}