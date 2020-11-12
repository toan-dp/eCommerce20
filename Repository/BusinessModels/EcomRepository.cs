using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.DomainModels;
using Repository.BusinessModels.ShopList;

namespace Repository.BusinessModels
{

    public class EcomRepository
    {
        // Business functions
        //

        private bool FullTextSearchFunction(string src, string value)
        {
            return src.ToLower().Contains(value.ToLower());
        }
        private bool PriceLimitCheck(int? minPrice, int? maxPrice, int val)
        {
            bool check = (minPrice == null) ? true : (val >= minPrice);
            check = (maxPrice == null) ? true : (val <= maxPrice);

            return check;
        }
        private bool IsSubCategory(int categoryId, Category parentCategory)
        {
            var node = parentCategory;
            while (node != null)
            {
                if (node.Id == categoryId)
                    return true;

                node = node.ParentCategory;
            }
            return false;
        }

        //
        // Queries
        //
        public Product GetProductById(int id)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductActivityTracking)
                    .FirstOrDefault(p => p.Id == id);
            }
        }
        public IEnumerable<Product> GetProducstByBrand(int brandId)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Products.Where(p => p.BrandId == brandId).ToList();
            }
        }
        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Products.Where(p => p.CategoryId == categoryId).ToList();
            }
        }
        public IEnumerable<ProductReview> GetProductReviews(int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.ProductReviews.Where(p => p.ProductId == productId).ToList();
            }
        }
        public IEnumerable<ShopListItem> GetShopList(ShopListOptions options, ShoppingCart cart, out int total)
        {
            List<Product> productList;
            using (EcomContext ecomContext = new EcomContext())
            {
                var optionCategory = ecomContext.Categories
                    .Include(p => p.ParentCategory)
                    .FirstOrDefault(p => p.Id == options.CategoryId);

                productList = ecomContext.Products
                .AsEnumerable()
                .Where(p => (options.CategoryId == null) ?      /*   Apply Filters   */
                                true : IsSubCategory(p.CategoryId, optionCategory))
                .Where(p => (options.BrandId == null) ?
                                true : (p.BrandId == options.BrandId))
                .Where(p => (options.SearchText == null) ?
                                true : (FullTextSearchFunction(p.ProductName, options.SearchText)
                                    || FullTextSearchFunction(p.Description, options.SearchText)))
                .Where(p => PriceLimitCheck(options.MinPrice, options.MaxPrice, p.UnitPrice))
                .OrderBy(p => p, new SortByComparer(options.SortBy))              /*   Apply Sort Options   */
                .ThenBy(p => p.UnitPrice, new PriceSortComparer(options.PriceSort))
                .ThenBy(p => p.ProductName, new AlphabetSortComparer(options.AlphabetSort))
                .ToList();

                total = productList.Count();        /* Get Total Matched */
                
                var shopList = productList
                    .Select(p => GetShopListItem(p, cart))
                    .Skip(((options.CurrentPage ?? 1) - 1) * (options.PageSize ?? 20))     /*  Pagination  */
                    .Take(options.PageSize ?? 20)
                    .ToList();
                return shopList;
            }
        }

        public ShopListItem GetShopListItem(Product product, ShoppingCart cart)
        {
            var shopListItem = new ShopListItem()
            {
                Product = product,
                IsAddedToCart = IsAddedToCart(product, cart)
            };
            return shopListItem;
        }

        public bool IsAddedToCart(Product product, ShoppingCart cart)
        {
            return cart.CartProducts
                .FirstOrDefault(p => p.ProductId == product.Id) != null;
        }

        public Brand GetBrandById(int id)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Brands.First(p => p.Id == id);
            }
        }
        public IEnumerable<Brand> GetAllBrands()
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Brands.ToList();
            }
        }

        public Category GetCategoryById(int? id)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Categories
                    .Include(p => p.ParentCategory)
                    .FirstOrDefault(p => p.Id == id);
            }
        }
        public IEnumerable<Category> GetParentCategories()
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.Categories.Include(p => p.ParentCategory).Where(p => p.ParentCategoryId == null).ToList();
            }
        }

        public void AddToCart(string cartId, int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                if (ecomContext.ShoppingCarts.FirstOrDefault(p => p.Id == cartId) == null)
                    return;

                var cartProduct = CartItem_GetByKeys(cartId, productId);
                bool isExist = cartProduct != null;

                if (isExist == true)
                    return;

                cartProduct = new CartProduct()
                {
                    Id = Guid.NewGuid().ToString(),
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = 1
                };
                ecomContext.CartProducts.Add(cartProduct);
                ecomContext.SaveChanges();
            }
        }

        public void RemoveFromCart(string cartId, int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                if (ecomContext.ShoppingCarts.FirstOrDefault(p => p.Id == cartId) == null)
                    return;
                var cartProduct = ecomContext.CartProducts
                   .FirstOrDefault(p => p.CartId == cartId
                                       && p.ProductId == productId);
                bool isExist = cartProduct != null;

                if (isExist == false)
                    return;

                ecomContext.CartProducts.Remove(cartProduct);
                ecomContext.SaveChanges();
            }
        }


        public void CartItem_IncreaseQuantity(string cartId, int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                if (ecomContext.ShoppingCarts.FirstOrDefault(p => p.Id == cartId) == null)
                    return;

                var cartProduct = ecomContext.CartProducts
                    .FirstOrDefault(p => p.CartId == cartId
                                        && p.ProductId == productId);
                bool isExist = cartProduct != null;

                if (isExist == false)
                    return;
                cartProduct.Quantity += 1;
                ecomContext.SaveChanges();
            }
        }
        public void CartItem_DecreaseQuantity(string cartId, int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                if (ecomContext.ShoppingCarts.FirstOrDefault(p => p.Id == cartId) == null)
                    return;

                var cartProduct = ecomContext.CartProducts
                   .FirstOrDefault(p => p.CartId == cartId
                                       && p.ProductId == productId);
                bool isExist = cartProduct != null;

                if (isExist == false)
                    return;

                cartProduct.Quantity = cartProduct.Quantity == 0 ?
                    0 : cartProduct.Quantity - 1;
                ecomContext.SaveChanges();
            }
        }

        public int GetCartTotalPrice(string cartId)
        {
            using (var context = new EcomContext())
            {
                return context.CartProducts
                    .Where(p => p.CartId == cartId)
                    .Sum(p => p.Product.UnitPrice * p.Quantity);
            }
        }

        public int GetCartTotalPrice(ShoppingCart cart)
        {
            using (var _db = new EcomContext())
            {
                int totalPrice = 0;
                foreach (CartProduct item in cart.CartProducts)
                {
                    Product p = _db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    totalPrice += (p != null) ?
                        p.UnitPrice * item.Quantity : 0;
                }
                return totalPrice;
            }
        }

        public ShoppingCart GetCartById(string id)
        {
            using (var context = new EcomContext())
            {
                return context.ShoppingCarts
                    .Include(p => p.CartProducts)
                    .FirstOrDefault(p => p.Id == id);
            }
        }

        public List<CartProduct> GetCartItems(ShoppingCart cart)        /* Tạm: không biết cách nào khác để lấy cartitem từ cookie tốt hơn */
        {
            using (var _db = new EcomContext())
            {
                var cartItems = new List<CartProduct>();
                foreach (var item in cart.CartProducts)
                {
                    var cartProduct = new CartProduct
                    {
                        Id = item.Id,
                        CartId = cart.Id,
                        Product = _db.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == item.ProductId),
                        Quantity = item.Quantity
                    };
                    cartItems.Add(cartProduct);
                }
                return cartItems;
            }
        }

        public CartProduct CartItem_GetByKeys(string cartId, int productId)
        {
            using (var ecomContext = new EcomContext())
            {
                return ecomContext.CartProducts
                    .FirstOrDefault(p => p.CartId == cartId
                        && p.ProductId == productId);
            }
        }

        public List<CartProduct> GetCartItemsByCartId(string cartId)
        {
            using (var context = new EcomContext())
            {
                return context.CartProducts
                    .Include(p => p.Product)
                    .Include(p => p.Cart)
                    .Where(p => p.CartId == cartId).ToList();
            }
        }

        public ShoppingCart ShoppingCart_Clone(string cartId, string newCartId)
        {
            using (var db = new EcomContext())
            {
                ShoppingCart cart = db.ShoppingCarts
                    .Include(p => p.CartProducts)
                    .FirstOrDefault(p => p.Id == cartId);

                ShoppingCart clone = new ShoppingCart { Id = newCartId, CartProducts = cart.CartProducts };
                return clone;
            }
        }

        public void ShoppingCart_Add(ShoppingCart cart)
        {
            using (var db = new EcomContext())
            {
                db.ShoppingCarts.Add(cart);
                var cartProducts = cart.CartProducts ?? new List<CartProduct>();
                foreach (var item in cartProducts)
                {
                    db.CartProducts.Add(new CartProduct
                    {
                        CartId = cart.Id,
                        Quantity = item.Quantity,
                        ProductId = item.ProductId
                    });
                }
                db.SaveChanges();
            }
        }

        public void ShoppingCart_Remove(string cartId)
        {
            using (var _db = new EcomContext())
            {
                var cart =_db.ShoppingCarts.FirstOrDefault(p => p.Id == cartId);
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
            }
        }
    }
}
