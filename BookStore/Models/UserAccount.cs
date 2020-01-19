using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookHive.Models
{
    public class UserAccount
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please enter the Email address in the correct format")]
        public string EmailId { get; set; }

        [Required]
        [DataType("Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType("Password")]
        public string RPassword { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
       
        public string ContactNo { get; set; }

        
        public string Role { get; set; }
    }
}