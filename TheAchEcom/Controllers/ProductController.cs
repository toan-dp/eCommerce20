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
using Repository.BusinessModels.ShopList;

namespace TheAchEcom.Controllers
{
    public class ProductDetailModel
    {
        public Product Product { get; set; }
        public bool IsAddedToCart { get; set; }

        public IEnumerable<ShopListItem> BrandAlso { get; set; }
        public IEnumerable<ShopListItem> CategoryAlso { get; set; }
        public IEnumerable<ProductReview> Reviews { get; set; }
    }

    public class ProductController : ApplicationController
    {

        private EcomRepository Repository = new EcomRepository();
        private SignInManager<Customer> SignInManager;
        private UserManager<Customer> UserManager;
        private IPageMaster PageMaster;

        public ProductController(
            IPageMaster pageMaster,
            SignInManager<Customer> signInManager,
            UserManager<Customer> userManager)
        {
            PageMaster = pageMaster;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public IActionResult ProductDetail(int id)
        {
            ShoppingCart cart = PageMaster.GetShoppingCart();
            Product product = Repository.GetProductById(id);

            var model = new ProductDetailModel();
            model.Product = product;
            model.IsAddedToCart = Repository.IsAddedToCart(product, cart);
            model.BrandAlso = Repository.GetProducstByBrand(product.BrandId)
                .Where(p => p.Id != product.Id)
                .Select(p => Repository.GetShopListItem(p, cart))
                .Take(8);

            model.CategoryAlso = Repository.GetProductsByCategory(product.CategoryId)
                .Where(p => p.Id != product.Id)
                .Select(p => Repository.GetShopListItem(p, cart))
                .Take(8);
            model.Reviews = Repository.GetProductReviews(product.Id);

            ViewBag.CountCartItems = cart.CartProducts.Count();
            return View(model);
        }
    }
}
