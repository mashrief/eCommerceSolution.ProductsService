using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;

namespace ProductsMicroService.API.APIEndpoints
{
    public static class ProductAPIEndpoints
    {
        public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products", async (IProductsService productService) =>
            {
                var products = await productService.GetProducts();
                return Results.Ok(products);
            });

            app.MapGet("/api/products/{ProductID:guid}", async (IProductsService productService, Guid ProductID) =>
            {
                var product = await productService.GetProductsByCondition(temp => temp.ProductID == ProductID);
                return Results.Ok(product);
            });

            app.MapPost("/api/product", async (IProductsService productService, IValidator<ProductAddRequest> productAddRequestValidator, ProductAddRequest productAddRequest) =>
            {
                var validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);
                
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.GroupBy(
                        error => error.PropertyName)
                        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
                    
                    return Results.ValidationProblem(errors);
                }
                
                var addedProduct = await productService.AddProduct(productAddRequest);

                if (addedProduct != null)
                {
                    return Results.Created($"/api/products/{addedProduct.ProductID}", addedProduct);
                }
                else
                {
                    return Results.Problem("An error occurred while adding the product.");
                }

            });

            app.MapPut("/api/product", async (IProductsService productService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
            {
                var validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
                
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.GroupBy(
                        error => error.PropertyName)
                        .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
                    
                    return Results.ValidationProblem(errors);
                }
                
                var updatedProduct = await productService.UpdateProduct(productUpdateRequest);
                
                if(updatedProduct != null)
                {
                    return Results.Ok(updatedProduct);
                }
                else
                {
                    return Results.Problem("An error occurred while updating the product.");
                }
            });

            app.MapDelete("/api/product/{ProductID:guid}", async (IProductsService productService, Guid ProductID) =>
            {
                var isDeleted = await productService.DeleteProduct(ProductID);
                
                if(isDeleted)
                {
                    return Results.Ok(true);
                }
                else
                {
                    return Results.Problem("An error occurred while deleting the product.");
                }
            });

            return app;
        }
    }
}
