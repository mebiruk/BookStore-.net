using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookHive.Models;
using System.Xml.Linq;

namespace BookHive.Controllers
{
    public class HomeController : Controller
    {

        
        public ActionResult Index()
        {
            //BookStore bs = new BookStore();
            return View(BookStore.Instance.GetBooks());
        }
        public ActionResult ActionAndAdventure()
        {
            return GetCategoryView("Action and Adventure");
            
        }
        public ActionResult LiteratureAndFiction()
        {
            return GetCategoryView("Literature And Fiction");
        }
        public ActionResult Biographies()
        {
            return GetCategoryView("Biographies");
        }
        public ActionResult InformationTechnology()
        {
            return GetCategoryView("Information Technology");
        }
        public ActionResult Childrens()
        {
            return GetCategoryView("Children's and Young Adults");
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        
        public ActionResult BookDetails(int id)
        {
            foreach (var i in BookStore.Instance.GetBooks())
            {
                if (id == i.BookId)
                {
                    return View(i);
                }
            }
            return View();
        }
        private ActionResult GetCategoryView(string CategoryName)
        {
            
            var results = from item in BookStore.Instance.GetBooks()
                          where item.Categories == CategoryName
                          select item;
            ViewBag.CategoryName = CategoryName;
            return View("Categories", results);
        }
        //[Authorize]
        //public ActionResult BuyNow()
        //{
        //    return View();
        //}
        [Authorize(Roles = "Admin")]
        
        public ActionResult AddBook()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddBook(book b)
        {
            //BookStore bs = new BookStore();
            BookStore.Instance.AddBooks(b);
            return RedirectToAction("Index","Home");
        }

        [Authorize(Roles="Admin")]
        public ActionResult Delete(int id)
        {
            foreach (var i in BookStore.Instance.GetBooks())
            {
                if (id == i.BookId)
                {
                    return View(i);
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(book b)
        {
            //BookStore bs = new BookStore();
            BookStore.Instance.DeleteBook(b);
            return RedirectToAction("Index","Home");
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id)
        {
            foreach (var i in BookStore.Instance.GetBooks())
            {
                if (id == i.BookId)
                {
                    return View(i);
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Update(book b)
        {
            //BookStore bs = new BookStore();
            BookStore.Instance.UpdateBook(b);
            return RedirectToAction("Index","Home");
        }
        [Authorize]
        public ActionResult AddToSession(int id,string ReturnUrl="")
        {
            if (Session["UserCart"] == null)
            {
                List<int> AddedBooks = new List<int>();
                AddedBooks.Add(id);
                Session["UserCart"] = AddedBooks;
            }
            else
            {
                var books = Session["UserCart"] as List<int>;
                books.Add(id);
            }
            return Redirect(ReturnUrl);
        }
        [Authorize]
        public ActionResult ViewCart()
        {
            var cart = Session["UserCart"] as List<int>;
            if (cart == null)
            {
                return View(new List<book>().AsEnumerable<book>());
            }
            else
            {
                var p = from b in BookStore.Instance.GetBooks()
                        where cart.Contains(b.BookId)
                        select b;
                return View(p);
            }
        }
        [Authorize]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = Session["UserCart"] as List<int>;
            cart.Remove(id);
            return RedirectToAction("ViewCart","Home");
        }
        [Authorize]
        public ActionResult PlaceOrder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PlaceOrder(int id)
        {
            var cart = Session["UserCart"] as List<int>;
            //HttpContext.Current.User.Identity.Name
            string Name= ControllerContext.HttpContext.User.Identity.Name;
            //BookStore bs = new BookStore();
            BookStore.Instance.PlaceOrder(Name, cart);
            Session["UserCart"] = null;
            return View(); 
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ViewOrderDetails()
        {
            //BookStore bs = new BookStore();
            
            return View(BookStore.Instance.ViewOrderDetails());
        }
    }
}
