using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookHive.Models
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        public bool RememberMe { get; set; }

        [Required]
        [DataType("Password")]
        public string Password { get; set; }
    }
}