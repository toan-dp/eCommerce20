using System;
using System.Collections.Generic;
using System.Text;
using Repository.DomainModels;
using System.Linq;

namespace Repository.BusinessModels.ShopList
{

    public class ShopListItem
    {
        public Product Product { get; set; }
        public bool IsAddedToCart { get; set; } = false;
    }

    public class ProductPriceLimit
    {
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public ProductPriceLimit(int min, int max)
        {
            MinPrice = min;
            MaxPrice = max;
        }

        public static ProductPriceLimit None()
        {
            return new ProductPriceLimit(0, 0);
        }

        public static IEnumerable<KeyValuePair<string, ProductPriceLimit>>
            GetAllPriceLimits()
        {
            var limits = new ProductPriceLimit[]
            {
                new ProductPriceLimit(50,100),
                new ProductPriceLimit(100,250),
                new ProductPriceLimit(250,400),
                new ProductPriceLimit(400,800)
            };

            var result = new List<KeyValuePair<string, ProductPriceLimit>>();
            foreach (var item in limits.Cast<ProductPriceLimit>())
            {
                string key = item.MinPrice.ToString() + " - " + item.MaxPrice.ToString();
                var _temp = new KeyValuePair<string, ProductPriceLimit>(key, item);
                result.Add(_temp);
            }
            return result;
        }
    }

    public enum PriceSortOptions
    {
        None = 19,
        LowPrice = 10,
        HighPrice = 11
    }

    public enum SortByOptions
    {
        None = 20,
        Newest = 12,
        Popularity = 13,
        BestSelling = 14
    }

    public enum AlphabetSortOptions
    {
        None = 18,
        Ascending = 15,
        Descending = 16
    }

    public class PriceSortComparer : IComparer<int>
    {
        public PriceSortOptions? SortOption { get; set; }
        public PriceSortComparer(PriceSortOptions? sortOption)
        {
            this.SortOption = sortOption;
        }
        public int Compare(int x, int y)
        {
            switch (this.SortOption)
            {
                case PriceSortOptions.LowPrice:
                    return (x - y);
                case PriceSortOptions.HighPrice:
                    return (y - x);
                case null:
                    break;
            }
            return 0;
        }
    }

    public class AlphabetSortComparer : IComparer<string>
    {
        public AlphabetSortOptions? SortOption { get; set; }
        public AlphabetSortComparer(AlphabetSortOptions? sortOption)
        {
            this.SortOption = sortOption;
        }
        public int Compare(string x, string y)
        {
            switch (this.SortOption)
            {
                case AlphabetSortOptions.Ascending:
                    return String.Compare(x, y);
                case AlphabetSortOptions.Descending:
                    return (-1) * String.Compare(x, y);
                case null:
                    break;
            }
            return 0;
        }
    }

    public class SortByComparer : IComparer<Product>
    {
        public SortByOptions? SortOption { get; set; }
        public SortByComparer(SortByOptions? sortOption)
        {
            this.SortOption = sortOption;
        }
        public int Compare(Product x, Product y)
        {
            switch (this.SortOption)
            {
                case SortByOptions.Newest:
                    return (-1) * DateTime.Compare(x.CreatedDate, y.CreatedDate);
                case SortByOptions.BestSelling:
                    {
                        var soldCompare = x.ProductActivityTracking.SoldCount - y.ProductActivityTracking.SoldCount;
                        var orderedCompare = x.ProductActivityTracking.OrderedCount - y.ProductActivityTracking.OrderedCount;

                        if (soldCompare != 0)
                            return soldCompare;
                        return orderedCompare;
                    }
                case SortByOptions.Popularity:
                    {
                        var viewCountCompare = x.ProductActivityTracking.ViewCount - y.ProductActivityTracking.ViewCount;
                        var ratingAvgCompare = x.ProductActivityTracking.RatingAvg - y.ProductActivityTracking.RatingAvg;
                        var reviewCountCompare = x.ProductActivityTracking.ReviewCount - -y.ProductActivityTracking.ReviewCount;
                        
                        if (reviewCountCompare != 0)
                            return reviewCountCompare;

                        if (ratingAvgCompare != 0)
                            return ratingAvgCompare;
                        
                        return viewCountCompare;
                    }
                case null:
                    break;
            }

            return 0;
        }
    }

    public class ShopListOptions
    {
        // Sorting Options
        public PriceSortOptions? PriceSort { get; set; }
        public SortByOptions? SortBy { get; set; }
        public AlphabetSortOptions? AlphabetSort { get; set; }

        // Filter Options
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string SearchText { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        // Pagination
        public int? CurrentPage { get; set; } = 1;
        public int? PageSize { get; set; } = 20;

        public ShopListOptions()
        {
            PriceSort = PriceSortOptions.None;
            SortBy = SortByOptions.None;
            AlphabetSort = AlphabetSortOptions.None;

            CurrentPage = 1;
            PageSize = 20;
        }
    }
}
