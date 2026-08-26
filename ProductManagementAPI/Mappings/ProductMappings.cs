using ProductManagementAPI.DTOs.Responses;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Mappings
{
    // A static class used to store product mapping methods.
    // Static means we do not need to create an object
    // of this class to use its methods.
    public static class ProductMappings
    {
        // Converts a Product entity into a ProductResponseDTO.
        //
        // The "this Product product" parameter makes this an extension method.
        // Therefore, it can be called directly on a Product object:
        // product.ToResponseDTO()
        public static ProductResponseDTO ToResponseDTO(this Product product)
        {
            // Creates a new response DTO and copies the required
            // property values from the Product entity.
            return new ProductResponseDTO
            {
                // Copies the unique product identifier.
                Id = product.Id,

                // Copies the product name.
                Name = product.Name,

                // Copies the Stock Keeping Unit.
                Sku = product.Sku,

                // Copies the optional product description.
                Description = product.Description,

                // Copies the product category.
                Category = product.Category,

                // Copies the product price.
                Price = product.Price,

                // Copies the available stock quantity.
                StockQuantity = product.StockQuantity,

                // Copies the active or inactive status.
                IsActive = product.IsActive,

                // Copies the product creation date and time.
                CreatedAt = product.CreatedAt,

                // Copies the optional last-updated date and time.
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
