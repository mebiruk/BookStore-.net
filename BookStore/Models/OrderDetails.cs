using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookHive.Models;
using System.ComponentModel.DataAnnotations;

namespace BookHive.Models
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; }
        public int BookId { get; set; }
        public DateTime OrderDate { get; set;}
    }
}