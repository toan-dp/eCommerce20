using System;
using System.Collections.Generic;
using Repository.DomainModels;
using Repository.BusinessModels;
using Repository.BusinessModels.ShopList;
using System.ComponentModel.DataAnnotations;

namespace TheAchEcom.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name="Your Name")]
        public string UserName { get; set; }
        
        [Required]
        [DataType("Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
