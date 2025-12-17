using ECommerce.Core.Enums;

namespace ECommerce.Core.Specifications.Orders
{
    public class OrderSpecParams
    {

        //Filter Params
        public OrderStatus? Status { get; set; }

        //Sort Params
        public string? Sort { get; set; }

        //Pagination Params
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 3;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 5) ? 5 : value;
        }
    }
}
