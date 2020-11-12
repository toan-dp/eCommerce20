using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheAchEcom.Models;
using Repository;
using Repository.DomainModels;
using Repository.BusinessModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text;

namespace TheAchEcom.Controllers
{
    public class CustomerController : ApplicationController
    {
        private EcomRepository Repository = new EcomRepository();
        private UserManager<Customer> UserManager { get; set; }
        private SignInManager<Customer> SignInManager { get; set; }

        public CustomerController(
            UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountModel model)
        {
            if (ModelState.IsValid == false)
                return View();

            var user = new Customer() { UserName = model.UserName, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                return RedirectToAction("CloneCartFromCookie", "ShoppingCart");
            }

            // error trying to create new user
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public async Task<IActionResult> LoginDefault()
        {
            var model = new LoginModel { UserName = "Huu", Password = "Huu@23419" };
            var result = await SignInManager
                .PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                HttpContext.Response.Cookies.Delete(_cartCookieName);
                return RedirectToAction("ShopList", "Shop");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid == false)
                return View();

            var result = await SignInManager
                .PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                HttpContext.Response.Cookies.Delete(_cartCookieName);
                return RedirectToAction("ShopList", "Shop");
            }

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await SignInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete(_cartCookieName);
            return RedirectToAction("ShopList", "Shop");
        }
        public static string GetFriendlyTitle(string title, bool remapToAscii = false, int maxlength = 80)
        {
            if (title == null)
            {
                return string.Empty;
            }

            int length = title.Length;
            bool prevdash = false;
            StringBuilder stringBuilder = new StringBuilder(length);
            char c;

            for (int i = 0; i < length; ++i)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    stringBuilder.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lower-case
                    stringBuilder.Append((char)(c | 32));
                    prevdash = false;
                }
                else if ((c == ' ') || (c == ',') || (c == '.') || (c == '/') ||
                  (c == '\\') || (c == '-') || (c == '_') || (c == '='))
                {
                    if (!prevdash && (stringBuilder.Length > 0))
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    int previousLength = stringBuilder.Length;

                    if (remapToAscii)
                    {
                        stringBuilder.Append(RemapInternationalCharToAscii(c));
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }

                    if (previousLength != stringBuilder.Length)
                    {
                        prevdash = false;
                    }
                }

                if (i == maxlength)
                {
                    break;
                }
            }

            if (prevdash)
            {
                return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }
            else
            {
                return stringBuilder.ToString();
            }
        }

        private static string RemapInternationalCharToAscii(char character)
        {
            string s = character.ToString().ToLowerInvariant();
            if ("àåáâäãåąā".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøő".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (character == 'ř')
            {
                return "r";
            }
            else if (character == 'ł')
            {
                return "l";
            }
            else if ("đð".Contains(s))
            {
                return "d";
            }
            else if (character == 'ß')
            {
                return "ss";
            }
            else if (character == 'Þ')
            {
                return "th";
            }
            else if (character == 'ĥ')
            {
                return "h";
            }
            else if (character == 'ĵ')
            {
                return "j";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
