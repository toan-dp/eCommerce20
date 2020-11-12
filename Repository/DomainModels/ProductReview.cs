using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.DomainModels
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public virtual DateTime CreatedDate { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("CustomerId")]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
