using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Common;
using ProductManagementAPI.DTOs.Requests;
using ProductManagementAPI.DTOs.Responses;
using ProductManagementAPI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementAPI.Controllers
{
    // Enables API-specific features such as automatic model validation
    // and automatic binding of incoming request values.
    [ApiController]

    // Defines the base URL for every action in this controller.
    // Base route: /api/products
    [Route("api/products")]

    public sealed class ProductsController(IProductService productService) : ControllerBase
    {
        // Stores the product service used by the controller.
        private readonly IProductService _productService = productService;

        // Handles GET requests to /api/products.
        [HttpGet]

        // Documents the possible HTTP responses for Swagger.
        [ProducesResponseType(
            typeof(ApiResponse<PagedResult<ProductResponseDTO>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDTO>>>> GetAll(
            [FromQuery] ProductQueryParameters queryParameters,

            // Allows the operation to be cancelled when
            // the HTTP request is cancelled.
            CancellationToken cancellationToken)
        {
            // Calls the service to retrieve the requested page of products.
            var products = await _productService.GetAllAsync(
                queryParameters,
                cancellationToken);

            // Creates a consistent successful API response.
            var response =
                ApiResponse<PagedResult<ProductResponseDTO>>.CreateSuccess(
                    StatusCodes.Status200OK,
                    "Products retrieved successfully.",
                    products,

                    // Adds the current request's unique trace identifier.
                    HttpContext.TraceIdentifier);

            // Returns HTTP 200 with the response body.
            return Ok(response);
        }

        // Handles GET requests such as /api/products/5.
        // The :int constraint ensures that the route value is an integer.
        [HttpGet("{id:int}")]

        // Documents the possible HTTP responses for Swagger.
        [ProducesResponseType(
            typeof(ApiResponse<ProductResponseDTO>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductResponseDTO>>> GetById(
            // Ensures that the product ID is greater than zero.
            [Range(
                1,
                int.MaxValue,
                ErrorMessage = "Product ID must be greater than zero.")]
            int id,

            CancellationToken cancellationToken)
        {
            // Calls the service to retrieve the product.
            // The service throws NotFoundException when it does not exist.
            var product = await _productService.GetByIdAsync(
                id,
                cancellationToken);

            // Creates a consistent successful API response.
            var response =
                ApiResponse<ProductResponseDTO>.CreateSuccess(
                    StatusCodes.Status200OK,
                    "Product retrieved successfully.",
                    product,
                    HttpContext.TraceIdentifier);

            // Returns HTTP 200 with the product information.
            return Ok(response);
        }

        // Handles POST requests to /api/products.
        [HttpPost]

        // Documents the possible HTTP responses for Swagger.
        [ProducesResponseType(
            typeof(ApiResponse<ProductResponseDTO>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductResponseDTO>>> Create(
            [FromBody] CreateProductRequestDTO request,
            CancellationToken cancellationToken)
        {
            // Calls the service to create and save the new product.
            var createdProduct = await _productService.CreateAsync(
                request,
                cancellationToken);

            // Creates a successful response containing the new product.
            var response =
                ApiResponse<ProductResponseDTO>.CreateSuccess(
                    StatusCodes.Status201Created,
                    "Product created successfully.",
                    createdProduct,
                    HttpContext.TraceIdentifier);

            // Returns HTTP 201 Created.
            // It also adds a Location header containing the URL
            // of the GetById action for the newly created product.
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                response);
        }

        // Handles PUT requests such as /api/products/5.
        [HttpPut("{id:int}")]

        // Documents the possible HTTP responses for Swagger.
        [ProducesResponseType(
            typeof(ApiResponse<ProductResponseDTO>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductResponseDTO>>> Update(
            // Ensures that the route ID is greater than zero.
            [Range(
                1,
                int.MaxValue,
                ErrorMessage = "Product ID must be greater than zero.")]
            int id,
            [FromBody] UpdateProductRequestDTO request,
            CancellationToken cancellationToken)
        {
            // Calls the service to update the specified product.
            var updatedProduct = await _productService.UpdateAsync(
                id,
                request,
                cancellationToken);

            // Creates a successful response containing the updated product.
            var response =
                ApiResponse<ProductResponseDTO>.CreateSuccess(
                    StatusCodes.Status200OK,
                    "Product updated successfully.",
                    updatedProduct,
                    HttpContext.TraceIdentifier);

            // Returns HTTP 200 with the updated product information.
            return Ok(response);
        }

        // Handles DELETE requests such as /api/products/5.
        [HttpDelete("{id:int}")]

        // Documents the possible HTTP responses for Swagger.
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(
            // Reads the product ID from the route.
            [FromRoute]

            // Ensures that the product ID is greater than zero.
            [Range(
                1,
                int.MaxValue,
                ErrorMessage = "Product ID must be greater than zero.")]
            int id,

            CancellationToken cancellationToken)
        {
            // Calls the service to delete the specified product.
            // The service throws NotFoundException if it does not exist.
            await _productService.DeleteAsync(
                id,
                cancellationToken);

            // Creates a successful response without returning data.
            var response =
                ApiResponse<object>.CreateSuccess(
                    StatusCodes.Status200OK,
                    "Product deleted successfully.",
                    data: null,
                    HttpContext.TraceIdentifier);

            // Returns HTTP 200 to confirm the deletion.
            return Ok(response);
        }
    }
}
