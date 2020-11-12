using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repository.DomainModels
{
    public class ProductActivityTracking
    { 
        public int Id { get; set; }
        // for Best Selling
        public int SoldCount { get; set; }
        public int OrderedCount { get; set; } 
        // for Popularity
        public int ViewCount { get; set; }
        public int ReviewCount { get; set; }
        public int RatingAvg { get; set; }
   
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
