using Repository.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheAchEcom.Models
{
    public class CartDetailModel
    {
        public ShoppingCart Cart { get; set; }
        public IEnumerable<CartProduct> CartItems { get; set; }
        public int CartTotalPrice { get; set; }
    }
}
