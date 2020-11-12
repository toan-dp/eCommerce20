using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheAchEcom.Models;
using Repository.DomainModels;
using Repository.BusinessModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace TheAchEcom.Controllers
{
    public class ShoppingCartController : ApplicationController
    {
        private EcomRepository Repository = new EcomRepository();
        private IPageMaster PageMaster;
        private UserManager<Customer> UserManager { get; set; }
        private SignInManager<Customer> SignInManager { get; set; }

        public ShoppingCartController(
            IPageMaster pageMaster,
            UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            PageMaster = pageMaster;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IActionResult CloneCartFromCookie()
        {
            try
            {
                string requestCookie = HttpContext.Request.Cookies[_cartCookieName];
                var cookieCart = JsonConvert.DeserializeObject<ShoppingCart>(requestCookie);

                cookieCart.Id = UserManager.GetUserId(User);
                Repository.ShoppingCart_Add(cookieCart);

                var responseCookie = JsonConvert.SerializeObject(cookieCart);
                HttpContext.Response.Cookies.Append(_cartCookieName, responseCookie);
            }
            catch (Exception)
            {
            }
            return RedirectToAction("ShopList", "Shop");
        }

        public IActionResult CartDetail()
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            var model = new CartDetailModel()
            {
                Cart = cart,
                CartItems = Repository.GetCartItems(cart),
                CartTotalPrice = Repository.GetCartTotalPrice(cart)
            };

            ViewBag.CountCartItems = cart.CartProducts.Count();
            return View(model);
        }

        [HttpPost]
        public JsonResult AddToCart(int productId)
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            if (SignInManager.IsSignedIn(User))
            {
                Repository.AddToCart(cart.Id, productId);
            }
            else   
            {
                /* 
                 * Add to Cookie Cart 
                 */

                bool isAddedToCart = cart.CartProducts
                    .FirstOrDefault(p => p.ProductId == productId) != null;
                if (isAddedToCart)
                    return new JsonResult(new { isSuccess = false });
                
                CartProduct cartItem = new CartProduct
                {
                    Id = Guid.NewGuid().ToString(),
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                cart.CartProducts.Add(cartItem);
                var responseCookie = JsonConvert.SerializeObject(cart);
                HttpContext.Response.Cookies.Append(_cartCookieName, responseCookie);
            }

            return new JsonResult(new
            {
                isSuccess = true,
                cartTotalPrice = Repository.GetCartTotalPrice(cart)
            });
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int productId)
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            if (SignInManager.IsSignedIn(User))
            {
                Repository.RemoveFromCart(cart.Id, productId);
            }
            else
            {
                /* 
                 * Remove From Cookie Cart 
                 */

                bool isAddedToCart = cart.CartProducts
                    .FirstOrDefault(p => p.ProductId == productId) != null;
                if (isAddedToCart == false)
                    return new JsonResult(new { isSuccess = false });

                var cartProducts = cart.CartProducts
                    .Where(p => p.ProductId != productId)
                    .ToList();
                cart.CartProducts = cartProducts;

                var responseCookie = JsonConvert.SerializeObject(cart);
                HttpContext.Response.Cookies.Append(_cartCookieName, responseCookie);
            }

            return new JsonResult(new
            {
                isSuccess = true,
                cartTotalItems = cart.CartProducts.Count(),
                cartTotalQuantity = cart.CartProducts.Sum(p => p.Quantity),
                cartTotalPrice = Repository.GetCartTotalPrice(cart)
            });
        }

        [HttpPost]
        public JsonResult IncreaseCartItemQuantity(int productId)
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            if (SignInManager.IsSignedIn(User))
            {
                Repository.CartItem_IncreaseQuantity(cart.Id, productId);
            }
            else
            {
                var cartItem = cart.CartProducts.FirstOrDefault(p => p.ProductId == productId);
                cartItem.Quantity = cartItem.Quantity + 1;

                var responseCookie = JsonConvert.SerializeObject(cart);
                HttpContext.Response.Cookies.Append(_cartCookieName, responseCookie);
            }
            
            return new JsonResult(new
            {
                isSuccess = true,
                cartTotalItems = cart.CartProducts.Count(),
                cartTotalQuantity = cart.CartProducts.Sum(p => p.Quantity),
                cartTotalPrice = Repository.GetCartTotalPrice(cart)
            });
        }

        [HttpPost]
        public JsonResult DecreaseCartItemQuantity(int productId)
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            if (SignInManager.IsSignedIn(User))
            {
                Repository.CartItem_DecreaseQuantity(cart.Id, productId);
            }
            else
            {
                var cartItem = cart.CartProducts.FirstOrDefault(p => p.ProductId == productId);
                var quantity = (cartItem.Quantity > 0) ? cartItem.Quantity - 1 : 0;
                cartItem.Quantity = quantity;

                var responseCookie = JsonConvert.SerializeObject(cart);
                HttpContext.Response.Cookies.Append(_cartCookieName, responseCookie);
            }

            return new JsonResult(new
            {
                isSuccess = true,
                cartTotalItems = cart.CartProducts.Count(),
                cartTotalQuantity = cart.CartProducts.Sum(p => p.Quantity),
                cartItemQuantity = cart.CartProducts
                                       .FirstOrDefault(p => p.ProductId == productId)
                                       .Quantity,
                cartTotalPrice = Repository.GetCartTotalPrice(cart)
            });
        }

        [HttpPost]
        public JsonResult CountCartItems()
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            return Json(new
            {
                isSuccess = true,
                total = cart.CartProducts.Count()
            });
        }

        public IActionResult MakeOrder()
        {
            // clear shopping cart
            var cart = PageMaster.GetShoppingCart();
            if (SignInManager.IsSignedIn(User))
            {
                Repository.ShoppingCart_Remove(cart.Id);
            }
            else
            {
                HttpContext.Response.Cookies.Delete(_cartCookieName);
            }
            return View();
        }
    }
}
