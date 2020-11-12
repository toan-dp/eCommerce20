using System;
using System.Collections.Generic;
using Repository.DomainModels;
using Repository.BusinessModels;
using Repository.BusinessModels.ShopList;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TheAchEcom.Models
{
    public interface IPageMaster
    {
        public ShoppingCart GetShoppingCart();
    }


    public class PageMaster : IPageMaster
    {
        private EcomRepository _repo = new EcomRepository();
        private HttpContext Context;
        private SignInManager<Customer> SignInManager;
        private UserManager<Customer> UserManager;
        private const string _cartCookieName = "_cartCookieName";

        public PageMaster(
            IHttpContextAccessor httpContextAccessor,
            SignInManager<Customer> signInManager,
            UserManager<Customer> userManager)
        {
            Context = httpContextAccessor.HttpContext;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public ShoppingCart GetShoppingCart()
        {
            ShoppingCart cart;
            if (SignInManager.IsSignedIn(Context.User))
            {
                string cartId = UserManager.GetUserId(Context.User);
                cart = _repo.GetCartById(cartId);
            }
            else
            {
                try
                {
                    string requestCookie = Context.Request.Cookies[_cartCookieName];
                    cart = JsonConvert.DeserializeObject<ShoppingCart>(requestCookie);
                }
                catch (Exception)
                {
                    cart = new ShoppingCart()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CartProducts = new List<CartProduct>()
                    };
                    Context.Response.Cookies
                        .Append(_cartCookieName, JsonConvert.SerializeObject(cart));
                }
            }

            return cart;
        }
    }
}
