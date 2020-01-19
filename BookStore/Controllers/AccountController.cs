using BookHive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookHive.Controllers
{
    public class AccountController : Controller
    {
      

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(Login l,string ReturnUrl = "")
        {
            //BookStore bs = new BookStore();
            if(ModelState.IsValid)
            {
                if (BookStore.Instance.GetLoggedIn(l, ReturnUrl))
                {
                    BookStore.Instance.SetupFormsAuthTicket(l.UserName,l.RememberMe);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserAccount ua,string ReturnUrl = "")
        {
            //BookStore bs = new BookStore();
            Login l = new Login();
            BookStore.Instance.AddUsers(ua);
            l.UserName = ua.UserName;
    
            if (ModelState.IsValid)
            {
                if (BookStore.Instance.GetLoggedIn(l,ReturnUrl))
                {
                    BookStore.Instance.SetupFormsAuthTicket(l.UserName, l.RememberMe);
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // implement SetFormsAuthTicket () and call it from login()
    }
}
