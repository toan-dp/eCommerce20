using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.DomainModels
{
    public class ShoppingCart
    {
        public string Id { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}
