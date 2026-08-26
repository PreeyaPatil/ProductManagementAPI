using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Common;
using ProductManagementAPI.Data;
using ProductManagementAPI.DTOs.Requests;
using ProductManagementAPI.Models;
using ProductManagementAPI.Repositories.Interfaces;

namespace ProductManagementAPI.Repositories.Implementations
{
    // Implements all product-related database operations
    // defined by the IProductRepository interface.
    public sealed class ProductRepository : IProductRepository
    {
        // Holds the EF Core DbContext used to access the database.
        private readonly ProductDbContext _dbContext;

        // Receives ProductDbContext through dependency injection.
        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Retrieves products after applying search, filtering,
        // sorting, and pagination.
        public async Task<PagedResult<Product>> GetAllAsync(
            ProductQueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            // Starts building a query for the Products table.
            // AsNoTracking is used because the products are only being read.
            IQueryable<Product> query =
                _dbContext.Products.AsNoTracking();

            // Applies search when the client provides a search term.
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                // Removes unnecessary spaces from the search term.
                var searchTerm = queryParameters.SearchTerm.Trim();

                // Searches for the value in the product name,
                // SKU, or category.
                query = query.Where(
                    product =>
                        product.Name.Contains(searchTerm) ||
                        product.Sku.Contains(searchTerm) ||
                        product.Category.Contains(searchTerm));
            }

            // Applies the category filter when a category is provided.
            if (!string.IsNullOrWhiteSpace(queryParameters.Category))
            {
                // Removes unnecessary spaces from the category.
                var category = queryParameters.Category.Trim();

                // Returns only products belonging to the given category.
                query = query.Where(
                    product => product.Category == category);
            }

            // Applies the active-status filter when a value is provided.
            if (queryParameters.IsActive.HasValue)
            {
                // Returns either active or inactive products.
                query = query.Where(
                    product =>
                        product.IsActive ==
                        queryParameters.IsActive.Value);
            }

            // Counts all matching records before pagination is applied.
            // This value is required to calculate the total number of pages.
            var totalRecords =
                await query.CountAsync(cancellationToken);

            // Executes the query and retrieves only the requested page.
            var products = await query
                // Sorts products by name to provide a consistent order.
                .OrderBy(product => product.Name)

                // Uses the ID as a secondary sort when names are the same.
                .ThenBy(product => product.Id)

                // Skips the records belonging to previous pages.
                .Skip(
                    (queryParameters.PageNumber - 1) *
                    queryParameters.PageSize)

                // Selects only the number of records required for the page.
                .Take(queryParameters.PageSize)

                // Executes the database query and returns the results as a list.
                .ToListAsync(cancellationToken);

            // Returns the products together with pagination information.
            return new PagedResult<Product>(
                products,
                queryParameters.PageNumber,
                queryParameters.PageSize,
                totalRecords);
        }

        // Retrieves a single product using its ID.
        public async Task<Product?> GetByIdAsync(
            int id,
            bool trackChanges,
            CancellationToken cancellationToken = default)
        {
            // Starts building a query for the Products table.
            IQueryable<Product> query = _dbContext.Products;

            // Disables change tracking when the product is needed
            // only for a read operation.
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            // Returns the matching product or null when it is not found.
            return await query.FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
        }

        // Checks whether the given SKU is already used by another product.
        public async Task<bool> ExistsBySkuAsync(
            string sku,
            int? excludedProductId = null,
            CancellationToken cancellationToken = default)
        {
            // AsNoTracking is used because this operation only checks
            // whether a matching record exists.
            return await _dbContext.Products
                .AsNoTracking()
                .AnyAsync(
                    product =>
                        // Checks for the matching SKU.
                        product.Sku == sku &&

                        // During an update, ignores the current product
                        // while checking for duplicate SKUs.
                        (!excludedProductId.HasValue ||
                         product.Id != excludedProductId.Value),
                    cancellationToken);
        }

        // Adds a new product to the EF Core change tracker.
        public async Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            // Marks the product as Added.
            // The INSERT occurs when SaveChangesAsync() is called.
            await _dbContext.Products.AddAsync(
                product,
                cancellationToken);
        }

        // Marks an existing product as modified.
        public void Update(Product product)
        {
            // The UPDATE occurs when SaveChangesAsync() is called.
            _dbContext.Products.Update(product);
        }

        // Marks an existing product for deletion.
        public void Delete(Product product)
        {
            // The DELETE occurs when SaveChangesAsync() is called.
            _dbContext.Products.Remove(product);
        }

        // Commits all pending additions, updates, and deletions
        // to the database.
        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            // Returns the number of database records affected.
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

