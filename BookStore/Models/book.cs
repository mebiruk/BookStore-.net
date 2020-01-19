using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookHive.Models
{
    public class book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string Categories { get; set; }
        public string Available { get; set; }
        public bool IsDiscounted { get; set; }
        public int DiscountPercent { get; set; }
        public float DiscountedPrice { get; set; }
        public string GetImageUrl()
        {
            return "/Images/Book_" + BookId + ".jpg";
        }
    }
}