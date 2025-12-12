using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Specifications
{
    public class ProductSpecParams
    {
        //Search Params
        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.ToLower(); // Force lowercase for consistency
        }

        //Filter Params
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Price cannot be negative.")]
        public decimal? MinPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Maximum Price cannot be negative.")]
        public decimal? MaxPrice { get; set; }

        //Sort Params
        public string? Sort { get; set; } // "priceAsc", "priceDesc", "name" , "newest arrivals", featured

        //Pagination Params
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 50) ? 50 : value;
        }



    }
}
