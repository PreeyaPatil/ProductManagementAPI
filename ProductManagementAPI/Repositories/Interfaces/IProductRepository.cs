using ProductManagementAPI.Common;
using ProductManagementAPI.DTOs.Requests;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetAllAsync(ProductQueryParameters queryParameters,
            CancellationToken cancellationToken=default);



        // Retrieves a product using its ID.
        // trackChanges determines whether EF Core should track the entity.
        // Tracking is normally enabled for update and delete operations.
        Task<Product?> GetByIdAsync(
            int id,
            bool trackChanges,
            CancellationToken cancellationToken = default);



        // Checks whether a product with the specified SKU already exists.
        // excludedProductId is used during an update to exclude
        // the product currently being updated from the duplicate check.
        Task<bool> ExistsBySkuAsync(
            string sku,
            int? excludedProductId = null,
            CancellationToken cancellationToken = default);


        // Adds a new product to the EF Core change tracker.
        // The product is not saved until SaveChangesAsync() is called.
        Task AddAsync(Product product, CancellationToken cancellationToken = default);



        // Marks an existing product as modified.
        void Update(Product product);

        // Marks an existing product for deletion.
        void Delete(Product product);

        // Saves all pending changes to the database.
        // It returns the number of affected database records.
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);

    }
}
