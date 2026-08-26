using ProductManagementAPI.Common;
using ProductManagementAPI.DTOs.Requests;
using ProductManagementAPI.DTOs.Responses;

namespace ProductManagementAPI.Services.Interfaces
{
    // Defines the business operations available for products.
    // The controller depends on this interface instead of directly
    // depending on the ProductService implementation.
    public interface IProductService
    {
        // Retrieves a paginated collection of products after applying
        // the requested search, filter, and pagination options.
        Task<PagedResult<ProductResponseDTO>> GetAllAsync(
            ProductQueryParameters queryParameters,
            CancellationToken cancellationToken = default);

        // Retrieves a single product using its ID.
        // It returns a response DTO instead of the database entity.
        Task<ProductResponseDTO> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Creates a new product using the submitted request data
        // and returns the created product.
        Task<ProductResponseDTO> CreateAsync(
            CreateProductRequestDTO request,
            CancellationToken cancellationToken = default);

        // Updates an existing product and returns the updated product.
        Task<ProductResponseDTO> UpdateAsync(
            int id,
            UpdateProductRequestDTO request,
            CancellationToken cancellationToken = default);

        // Deletes an existing product using its ID.
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}

