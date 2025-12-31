namespace ECommerce.Business.DTOs.Reviews.Responses
{
    public class ReviewSummaryDto
    {
        public int Id { get; set; }
        public string ProductThumbnailUrl { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? Comment { get; set; }
        public int HelpfulCount { get; set; }
    }

}
