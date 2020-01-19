using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using BookHive.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Policy;
using System.Globalization;
using System.Xml.Linq;

namespace BookHive.Models
{
    public class BookStore
    {
        #region Singleton
        private static BookStore instance = null;

        private BookStore()
        {
        }

        public static BookStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookStore();
                }
                return instance;
            }
        }
        #endregion
        private string ConnectionString = "Data Source=.; Initial Catalog=BookHive;Integrated Security=SSPI";
        int discountPercent;
        string discountCategory;
        public List<book> GetBooks()
        {

            string discountFilePath = HttpContext.Current.Server.MapPath("/App_Data/DiscountSettings.xml");
            XDocument xmlDoc = XDocument.Load(discountFilePath);
            IEnumerable<XElement> filteredDiscounts;
            filteredDiscounts = from d in xmlDoc.Descendants("Discount")
                                where Convert.ToDateTime(d.Element("StartDate").Value)  < DateTime.Now 
                                && DateTime.Parse(d.Element("EndDate").Value) >= DateTime.Now
                                select d;
            
            XElement discountElement = filteredDiscounts.LastOrDefault();
            if (discountElement != null)
            {
                discountCategory = discountElement.Element("GenreName").Value;
                discountPercent = Convert.ToInt32(discountElement.Element("DiscountPercentage").Value);
            }
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select * from books", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<book> BookCollection = new List<book>();
            while (dr.Read())
            {
                book b = new book();
                b.BookId = Convert.ToInt32(dr["BookId"]);
                b.BookName = Convert.ToString(dr["BookName"]);
                b.AuthorName = Convert.ToString(dr["AuthorName"]);
                b.Price = Convert.ToInt32(dr["Price"]);
                b.Description = Convert.ToString(dr["Descriptions"]);
                b.Categories = Convert.ToString(dr["Categories"]);
                if (b.Categories == discountCategory)
                {
                    b.IsDiscounted = true;
                    b.DiscountPercent = discountPercent;
                    b.DiscountedPrice = b.Price * (1 - b.DiscountPercent / 100.0f);
                }
                else
                {
                    b.IsDiscounted = false;
                    b.DiscountPercent = 0;
                    b.DiscountedPrice = b.Price;
                }
                BookCollection.Add(b);
            }

            con.Close();

            return BookCollection;
        }
        public void AddUsers(UserAccount u)
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Insert Into UserAccount(UserName,EmailId,UserPassword,UserAddress,City,UserState,ContactNum) values('" + u.UserName + "','" + u.EmailId + "','"
                + u.Password + "','" + u.Address + "','" + u.City + "','" + u.State + "','" + u.ContactNo + "')", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public bool GetLoggedIn(Login l, string ReturnUrl = "")
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select UserName,UserPassword from UserAccount", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (l.UserName == Convert.ToString(dr["UserName"]) && l.Password == Convert.ToString(dr["UserPassword"]))
                {

                    return true;
                }
            }
            return false;
        }
        public UserAccount SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
            UserAccount u = new UserAccount();
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select UserName,UserPassword from UserAccount", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                u.UserName = Convert.ToString(dr["UserName"]);
            }
            var userId = u.UserId;
            var userData = userId.ToString(CultureInfo.InvariantCulture);

            var authTicket = new FormsAuthenticationTicket(1, //version
                                userName, // user name
                                DateTime.Now,             //creation
                                DateTime.Now.AddMinutes(30), //Expiration
                                persistanceFlag,//Persistent
                                userData
                                );
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return u;
        }
        public void AddBooks(book b)
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            string command = "Insert Into books(BookName,AuthorName,Price,Descriptions,Categories) values('" + b.BookName + "','" + b.AuthorName + "',"
                + b.Price + ",'" + b.Description.Replace("'", "''") + "','" + b.Categories + "')";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateBook(book b)
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Update books set BookName = '" + b.BookName + "', AuthorName = '" + b.AuthorName + "',Price ="
                + b.Price + ",Descriptions = '" + b.Description.Replace("'", "''") + "', Categories = '" + b.Categories.Replace("'", "''") + "', Available = '" + b.Available + "' where bookid=" + b.BookId, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void PlaceOrder(string UserName, List<int> BookId)
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("select UserId from UserAccount where UserName = '" + UserName + "'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            int UserId = 0;
            while (dr.Read())
            {
                UserId = Convert.ToInt32(dr["UserId"]);
            }
            con.Close();
            con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd1 = new SqlCommand("Insert into Orders(UserId,OrderDate) values(" + UserId + ",GetDate())", con);
            con.Open();
            cmd1.ExecuteNonQuery();
            con.Close();
            con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd2 = new SqlCommand("Select max(OrderId) as OrderId from Orders where UserId =" + UserId, con);
            con.Open();
            SqlDataReader dr1 = cmd2.ExecuteReader();
            int OrderId = 0;
            while (dr1.Read())
            {
                OrderId = Convert.ToInt32(dr1["OrderId"]);
            }
            con.Close();

            var books = GetBooks().Where(i => BookId.Contains(i.BookId));

            foreach (var i in books)
            {
                con = new SqlConnection(this.ConnectionString);
                SqlCommand cmd3 = new SqlCommand("Insert into OrderDetails(OrderId,BookId,BookPrice) values(" + OrderId + "," + i.BookId + ","+i.DiscountedPrice+")", con);
                con.Open();
                cmd3.ExecuteNonQuery();
                con.Close();
            }
        }
        public List<OrderDetails> ViewOrderDetails()
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("select o.OrderId,u.UserName,b.BookName,d.BookId,o.OrderDate from Orders o join OrderDetails d on o.OrderId = d.OrderId join UserAccount u on o.UserId = u.UserId join books b on d.BookId = b.BookId", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<OrderDetails> Orders = new List<OrderDetails>();
            while (dr.Read())
            {
                OrderDetails o = new OrderDetails();
                o.OrderId = Convert.ToInt32(dr["OrderId"]);
                o.UserName = Convert.ToString(dr["UserName"]);
                o.BookName = Convert.ToString(dr["BookName"]);
                o.BookId = Convert.ToInt32(dr["BookId"]);
                o.OrderDate = Convert.ToDateTime(dr["OrderDate"]);
                Orders.Add(o);
            }
            return Orders;
        }
        public void DeleteBook(book b)
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Delete from books where BookId =" + b.BookId, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }

}