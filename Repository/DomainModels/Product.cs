using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repository.DomainModels
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int UnitPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; }
        public string FlugName { get; set; }

        //[ForeignKey("MainImageId")]
        //public int ImageId { get; set; }
        //public ProductImage MainImage { get; set; }
        //public ICollection<ProductImage> ProductImages { get; set; }

        [ForeignKey("BrandId")]
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        
        public virtual ProductActivityTracking ProductActivityTracking { get; set; }
    }
}
