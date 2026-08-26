using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Data;
using ProductManagementAPI.Handlers;
using ProductManagementAPI.Repositories.Implementations;
using ProductManagementAPI.Repositories.Interfaces;
using ProductManagementAPI.Services.Implementations;
using ProductManagementAPI.Services.Interfaces;

namespace ProductManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registers controller support with dependency injection.
            builder.Services
                .AddControllers()

                // Configures JSON serialization options.
                .AddJsonOptions(options =>
                {
                    // Disables the default camelCase conversion.
                    // C# property names will remain unchanged in JSON.
                    // For example, ProductName remains ProductName.
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            // Configure JSON written directly through
            // HttpResponse.WriteAsJsonAsync(), such as the response
            // written by GlobalExceptionHandler.
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                // Keep C# property names in JSON.
                options.SerializerOptions.PropertyNamingPolicy = null;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registers ProductDbContext with dependency injection.
            // EF Core will use SQL Server and the ProductDbConnection
            // connection string from the configuration file.
            builder.Services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "ProductDbConnection")));

            // Registers the repository with a scoped lifetime.
            // A new ProductRepository instance is created for each request.
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            // Registers the service with a scoped lifetime.
            // When IProductService is requested, ProductService is provided.
            builder.Services.AddScoped<IProductService, ProductService>();

            // Registers the custom global exception handler.
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Register the default fallback error-response service.
            // The custom GlobalExceptionHandler runs first.
            // ProblemDetails is used only as a fallback if no custom
            // exception handler handles the exception.
            builder.Services.AddProblemDetails();

            // Reads the AllowedOrigins array from appsettings.json.
            var allowedOrigins =
                builder.Configuration
                    .GetSection("AllowedOrigins")
                    .Get<string[]>();

            // Registers a CORS policy that allows the Angular application
            // to communicate with this Web API.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAngularClient",
                    policy =>
                    {
                        policy
                            // Allows requests only from these origins.
                            // AllowAnyOrigin() can be used when every
                            // origin should be allowed.
                            .WithOrigins(allowedOrigins ?? Array.Empty<string>())

                            // Allows any HTTP request header,
                            // such as Content-Type and Authorization.
                            .AllowAnyHeader()

                            // Allows any HTTP method,
                            // such as GET, POST, PUT, and DELETE.
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Enables the registered Global Exception Handler.
            // It catches unhandled exceptions and passes them
            // to GlobalExceptionHandler.
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Enables the registered CORS policy globally.
            app.UseCors("AllowAngularClient");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
