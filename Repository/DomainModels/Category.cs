using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repository.DomainModels
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
   
        [ForeignKey("ParentCategoryId")]
        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }
    
        public virtual ICollection<Product> Products { get; set; }
    }
}
