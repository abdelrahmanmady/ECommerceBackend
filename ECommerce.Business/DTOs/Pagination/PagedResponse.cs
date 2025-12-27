namespace ECommerce.Business.DTOs.Pagination
{
    public class PagedResponse<T>() //Output
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public IEnumerable<T> Items { get; set; } = [];
    }
}
