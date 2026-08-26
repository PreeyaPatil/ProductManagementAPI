using System.ComponentModel.DataAnnotations;
namespace ProductManagementAPI.DTOs.Requests
{
    public class CreateProductRequestDTO
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 2, 
            ErrorMessage = "Product name must be between 2 and 150 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage="SKU required.")]
        [StringLength(50, MinimumLength = 2,ErrorMessage="SKU must be between 2 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9\-]*$", ErrorMessage = "SKU can contain letters, numbers, and hyphens only.")]
        public string Sku { get; set; } = string.Empty;
        [StringLength(1000,ErrorMessage="Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category required.")]
       [StringLength(10500, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 100 characters.")]
        public string Category { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Price must be greater than zero.")]

        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]

        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
