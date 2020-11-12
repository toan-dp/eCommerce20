using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.DomainModels
{
    public class CartProduct
    {
        public string Id { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        
        [ForeignKey("CartId")]
        public string CartId { get; set; }
        public virtual ShoppingCart Cart { get; set; }
    }
}
