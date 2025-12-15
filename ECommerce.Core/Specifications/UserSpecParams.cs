namespace ECommerce.Core.Specifications
{
    public class UserSpecParams
    {
        //Filter Params
        public string? Role { get; set; } //Admin , Customer

        //Search Params
        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.ToLower(); // Force lowercase for consistency
        }

        //Sort Params 
        public string? Sort { get; set; } // newestfirst , oldestfirst, name , email 

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
