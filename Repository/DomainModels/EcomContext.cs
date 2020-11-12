using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Repository.DomainModels
{

    public class EcomContext: IdentityDbContext<Customer>
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductActivityTracking> ProductActivityTrackings { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public EcomContext() { }
        public EcomContext(DbContextOptions<EcomContext> options):base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = @"data source=.\SQLEXPRESS;";
            connectionString += "initial catalog=TheAchEcom;";
            connectionString += "integrated security=true;";

            builder.UseSqlServer(connectionString);
            builder.UseLazyLoadingProxies();
            base.OnConfiguring(builder);
        }
    }
}
