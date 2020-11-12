using System;
using System.Collections.Generic;
using Repository.DomainModels;
using Repository.BusinessModels;
using Repository.BusinessModels.ShopList;
using System.ComponentModel.DataAnnotations;

namespace TheAchEcom.Models
{
    public class AccountModel
    {
        [Required]
        [Display(Name = "Your name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name="User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Your email")]
        public string Email { get; set; }

        [Required]
        [DataType("Password")]
        public string Password { get; set; }

        [Required]
        [DataType("Password")]
        [Display(Name = "Password confirmation")]
        [Compare("Password", ErrorMessage = "Password confirmation is not matched!!")]
        public string ConfirmPassword { get; set; }

        [Display(Name="Keep me login")]
        public bool RememberMe { get; set; } = false;
    }
}
