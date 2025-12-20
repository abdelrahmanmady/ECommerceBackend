using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Categories.Admin
{
    public class AdminUpdateCategoryDto
    {
        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(50, ErrorMessage = "Category Name cannot exceed 50 characters")]
        public string Name { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        public int? ParentId { get; set; }
    }
}
