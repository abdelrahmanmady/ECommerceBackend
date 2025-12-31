using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Reviews.Requests
{
    public class AddReviewRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200, ErrorMessage = "Review text cannot exceed 500 chars.")]
        public string? Comment { get; set; }
    }

}
