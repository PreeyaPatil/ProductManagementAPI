using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Models;
namespace ProductManagementAPI.Data
{
    // Represents the application's database session.
    // It is used to query and save Product data.
    public sealed class ProductDbContext : DbContext
    {
        // Receives database configuration through dependency injection.
        // The configuration includes details such as the connection string
        // and the database provider.
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        // Represents the Products table in the database.
        // It is used to query, add, update, and delete Product entities.
        public DbSet<Product> Products { get; set; }

        // Configures how the Product entity should be mapped
        // to the database table and columns.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Applies the default configurations provided by DbContext.
            base.OnModelCreating(modelBuilder);

            // Starts the database configuration for the Product entity.
            modelBuilder.Entity<Product>(entity =>
            {
                // Maps the Product entity to the Products table.
                entity.ToTable("Products");

                // Configures Id as the primary key.
                entity.HasKey(product => product.Id);

                // Configures Id to be generated automatically
                // when a new product is inserted.
                entity.Property(product => product.Id)
                      .ValueGeneratedOnAdd();

                // Makes Name required with a maximum length of 150 characters.
                entity.Property(product => product.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                // Makes SKU required with a maximum length of 50 characters.
                entity.Property(product => product.Sku)
                      .IsRequired()
                      .HasMaxLength(50);

                // Makes Description optional with a maximum length
                // of 1,000 characters.
                entity.Property(product => product.Description)
                      .HasMaxLength(1000);

                // Makes Category required with a maximum length
                // of 100 characters.
                entity.Property(product => product.Category)
                      .IsRequired()
                      .HasMaxLength(100);

                // Stores Price with 18 total digits and 2 decimal places.
                entity.Property(product => product.Price)
                      .HasPrecision(18, 2);

                // Makes StockQuantity a required column.
                entity.Property(product => product.StockQuantity)
                      .IsRequired();

                // Sets the default value of IsActive to true.
                entity.Property(product => product.IsActive)
                      .HasDefaultValue(true);

                // Stores CreatedAt using SQL Server's datetime2 type.
                // SQL Server automatically assigns the current UTC date
                // and time when a record is created.
                entity.Property(product => product.CreatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                // Stores the optional last-updated date and time
                // using SQL Server's datetime2 type.
                entity.Property(product => product.UpdatedAt)
                      .HasColumnType("datetime2");

                // Creates a unique index on SKU.
                // This prevents two products from having the same SKU.
                entity.HasIndex(product => product.Sku)
                      .IsUnique()
                      .HasDatabaseName("UX_Products_Sku");

                // Creates an index on Category to improve the performance
                // of queries that filter products by category.
                entity.HasIndex(product => product.Category)
                      .HasDatabaseName("IX_Products_Category");

                // Creates an index on IsActive to improve the performance
                // of queries that filter active or inactive products.
                entity.HasIndex(product => product.IsActive)
                      .HasDatabaseName("IX_Products_IsActive");
            });

            // Seeds initial products into the Products table.
            // Fixed IDs and dates are used because seed data must be
            // consistent whenever EF Core generates migrations.
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Sku = "LAPTOP-001",
                    Description = "15-inch business laptop",
                    Category = "Electronics",
                    Price = 60000.00m,
                    StockQuantity = 20,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Sku = "PHONE-001",
                    Description = "5G Android smartphone",
                    Category = "Electronics",
                    Price = 30000.00m,
                    StockQuantity = 40,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                },
                new Product
                {
                    Id = 3,
                    Name = "Wireless Mouse",
                    Sku = "MOUSE-001",
                    Description = "Wireless optical mouse",
                    Category = "Accessories",
                    Price = 1500.00m,
                    StockQuantity = 100,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                },
                new Product
                {
                    Id = 4,
                    Name = "Mechanical Keyboard",
                    Sku = "KEYBOARD-001",
                    Description = "Mechanical keyboard with backlight",
                    Category = "Accessories",
                    Price = 4500.00m,
                    StockQuantity = 50,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                },
                new Product
                {
                    Id = 5,
                    Name = "LED Monitor",
                    Sku = "MONITOR-001",
                    Description = "24-inch Full HD LED monitor",
                    Category = "Electronics",
                    Price = 18000.00m,
                    StockQuantity = 30,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                }
            );
        }
    }
}
