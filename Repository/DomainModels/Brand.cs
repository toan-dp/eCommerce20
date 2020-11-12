using System;
using System.Collections.Generic;

namespace Repository.DomainModels
{
    public class Brand
    {
        public int Id { get; set; }
        public string BrandName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Brand() { }
    }
}
