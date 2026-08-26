using ProductManagementAPI.Exceptions;
using ProductManagementAPI.Common;
using ProductManagementAPI.DTOs.Requests;
using ProductManagementAPI.DTOs.Responses;
using ProductManagementAPI.Mappings;
using ProductManagementAPI.Models;
using ProductManagementAPI.Repositories.Interfaces;
using ProductManagementAPI.Services.Interfaces;

namespace ProductManagementAPI.Services.Implementations
{
    // Contains the business logic for managing products.
    // It coordinates repository operations, performs business checks,
    // and converts Product entities into response DTOs.
    public sealed class ProductService : IProductService
    {
        // Stores the repository used to perform database operations.
        private readonly IProductRepository _productRepository;

        // Receives the repository through dependency injection.
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Retrieves a paginated collection of products.
        public async Task<PagedResult<ProductResponseDTO>> GetAllAsync(
            ProductQueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            // Gets the filtered and paginated Product entities
            // from the repository.
            var pagedProducts =
                await _productRepository.GetAllAsync(
                    queryParameters,
                    cancellationToken);

            // Converts each Product entity into a ProductResponseDTO.
            var productDTOs = pagedProducts.Items
                .Select(product => product.ToResponseDTO())
                .ToList();

            // Returns the converted products with the original
            // pagination information.
            return new PagedResult<ProductResponseDTO>(
                productDTOs,
                pagedProducts.PageNumber,
                pagedProducts.PageSize,
                pagedProducts.TotalRecords);
        }

        // Retrieves a single product using its ID.
        public async Task<ProductResponseDTO> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            // Retrieves the product without change tracking because
            // this is a read-only operation.
            var product = await _productRepository.GetByIdAsync(
                id,
                trackChanges: false,
                cancellationToken);

            // Throws a custom exception when the product does not exist.
            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            // Converts the entity into a response DTO.
            return product.ToResponseDTO();
        }

        // Creates and saves a new product.
        public async Task<ProductResponseDTO> CreateAsync(
            CreateProductRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            // Removes spaces from the SKU and converts it to uppercase.
            var normalizedSku = NormalizeSku(request.Sku);

            // Checks whether another product already uses the same SKU.
            var skuExists =
                await _productRepository.ExistsBySkuAsync(
                    normalizedSku,
                    cancellationToken: cancellationToken);

            // Duplicate SKUs are not allowed.
            if (skuExists)
            {
                throw new BadRequestException(
                    $"A product with SKU '{normalizedSku}' already exists.");
            }

            // Creates a new Product entity from the request DTO.
            var product = new Product
            {
                // Removes unnecessary spaces from the product name.
                Name = request.Name.Trim(),

                // Stores the normalized SKU.
                Sku = normalizedSku,

                // Converts empty descriptions to null and trims valid text.
                Description =
                    NormalizeOptionalText(request.Description),

                // Removes unnecessary spaces from the category.
                Category = request.Category.Trim(),

                // Copies the remaining values from the request DTO.
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsActive = request.IsActive,

                // Records the product creation time in UTC.
                CreatedAt = DateTime.UtcNow
            };

            // Adds the new product to the EF Core change tracker.
            await _productRepository.AddAsync(
                product,
                cancellationToken);

            // Saves the new product to the database.
            await _productRepository.SaveChangesAsync(
                cancellationToken);

            // Returns the newly created product as a response DTO.
            return product.ToResponseDTO();
        }

        // Updates an existing product.
        public async Task<ProductResponseDTO> UpdateAsync(
            int id,
            UpdateProductRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            // Retrieves the product with change tracking enabled
            // because its property values will be modified.
            var product = await _productRepository.GetByIdAsync(
                id,
                trackChanges: true,
                cancellationToken);

            // Stops the operation when the product does not exist.
            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            // Normalizes the SKU received from the client.
            var normalizedSku = NormalizeSku(request.Sku);

            // Checks whether another product already uses the new SKU.
            // The current product ID is excluded from the check.
            var skuExists =
                await _productRepository.ExistsBySkuAsync(
                    normalizedSku,
                    excludedProductId: id,
                    cancellationToken);

            // Stops the update when another product uses the same SKU.
            if (skuExists)
            {
                throw new BadRequestException(
                    $"Another product with SKU '{normalizedSku}' already exists.");
            }

            // Copies the updated values into the existing Product entity.
            product.Name = request.Name.Trim();
            product.Sku = normalizedSku;
            product.Description =
                NormalizeOptionalText(request.Description);
            product.Category = request.Category.Trim();
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.IsActive = request.IsActive;

            // Records when the product was last updated.
            product.UpdatedAt = DateTime.UtcNow;

            // Marks the product as modified.
            _productRepository.Update(product);

            // Saves the updated values to the database.
            await _productRepository.SaveChangesAsync(
                cancellationToken);

            // Returns the updated product as a response DTO.
            return product.ToResponseDTO();
        }

        // Deletes a product using its ID.
        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            // Retrieves the product with tracking enabled because
            // it will be removed from the database.
            var product = await _productRepository.GetByIdAsync(
                id,
                trackChanges: true,
                cancellationToken);

            // Stops the operation when the product does not exist.
            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            // Marks the product for deletion.
            _productRepository.Delete(product);

            // Executes the DELETE operation in the database.
            await _productRepository.SaveChangesAsync(
                cancellationToken);
        }

        // Standardizes an SKU before storing or comparing it.
        private static string NormalizeSku(string sku)
        {
            // Removes spaces from both ends and converts the value
            // to uppercase using culture-independent rules.
            return sku.Trim().ToUpperInvariant();
        }

        // Standardizes optional text such as a description.
        private static string? NormalizeOptionalText(string? value)
        {
            // Returns null when the value is null, empty, or whitespace.
            // Otherwise, it returns the value without surrounding spaces.
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}
