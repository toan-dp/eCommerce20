using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheAchEcom.Models;
using Repository.DomainModels;
using Repository.BusinessModels.ShopList;
using Repository.BusinessModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace TheAchEcom.Controllers
{
    public class ShopController : ApplicationController
    {
        private EcomRepository Repository = new EcomRepository();
        private SignInManager<Customer> SignInManager;
        private UserManager<Customer> UserManager;

        public ShopController(SignInManager<Customer> signInManager, UserManager<Customer> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public IActionResult ShopList(ShopListOptions options)
        {
            ShoppingCart cart;
            if (SignInManager.IsSignedIn(User))
            {
                string cartId = UserManager.GetUserId(User);
                cart = Repository.GetCartById(cartId);
            }
            else
            {
                try
                {
                    
                    string requestCookie = HttpContext.Request.Cookies[_cartCookieName];
                    cart = JsonConvert.DeserializeObject<ShoppingCart>(requestCookie);
                }
                catch (Exception)
                {
                    cart = new ShoppingCart()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CartProducts = new List<CartProduct>()
                    };
                    HttpContext.Response.Cookies
                        .Append(_cartCookieName, JsonConvert.SerializeObject(cart));
                }
            }

            var model = new ShopListModel();
            int total;
            model.ShopList = Repository.GetShopList(options, cart, out total);
            model.Brands = Repository.GetAllBrands();
            model.TotalPage = total / (options.PageSize??20);

            ViewBag.Options = options;
            ViewBag.CountCartItems = cart.CartProducts.Count();
            return View(model);
        }
    }
}
