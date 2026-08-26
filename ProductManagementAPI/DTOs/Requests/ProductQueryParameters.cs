using System.ComponentModel.DataAnnotations;

namespace ProductManagementAPI.DTOs.Requests
{
    public class ProductQueryParameters
    {
        [StringLength(150, ErrorMessage="Search term cannot exceed 150 characters.")]
        public string? SearchTerm { get; set; }
        [StringLength(100,ErrorMessage="Category cannot exceed 100 characters.")]
        public string? Category { get; set; }

        public bool? IsActive { get; set; }
        [Range(1,int.MaxValue,ErrorMessage="Page number must be at least 1.")]
        public int PageNumber { get; set; }=1;
        [Range(1, 100, ErrorMessage = "page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

    }
}
