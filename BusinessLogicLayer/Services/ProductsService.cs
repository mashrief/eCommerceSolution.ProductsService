using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;

        public ProductsService
        (
            IValidator<ProductAddRequest> productAddRequestValidator, 
            IValidator<ProductUpdateRequest> productUpdateRequestValidator,
            IMapper mapper,
            IProductsRepository productsRepository
        ) 
        {
            _productAddRequestValidator = productAddRequestValidator;
            _productUpdateRequestValidator = productUpdateRequestValidator;
            _mapper = mapper;
            _productsRepository = productsRepository;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            if(productAddRequest == null)
            {
                throw new ArgumentNullException(nameof(productAddRequest));
            }

            var validationResult = _productAddRequestValidator.Validate(productAddRequest);

            if(!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var addedProduct  = _mapper.Map<Product>(productAddRequest);

            await _productsRepository.AddProduct(addedProduct);

            if(addedProduct == null)
            {
                return null;
            }

            var addedProductResponse = _mapper.Map<ProductResponse>(addedProduct);

            return addedProductResponse;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            var existingProduct = _productsRepository.GetProductsByCondition(temp => temp.ProductID == productID);
            
            if (existingProduct == null)
            {
                return false;
            }

            var isDeleted = await _productsRepository.DeleteProduct(productID);

            return isDeleted;
        }

        public async Task<List<ProductResponse?>> GetProducts()
        {
            var products = await _productsRepository.GetProducts();

            var productsResponse = _mapper.Map<IEnumerable<ProductResponse?>>(products);

            return productsResponse.ToList();
        }

        public async Task<ProductResponse?> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            var product = await _productsRepository.GetProductsByCondition(conditionExpression);

            if(product == null) 
            {
                return null;
            }

            var productResponse = _mapper.Map<ProductResponse>(product);

            return productResponse;
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            var existingProduct = await _productsRepository.GetProductsByCondition(temp => temp.ProductID == productUpdateRequest.ProductID);

            if (existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {productUpdateRequest.ProductID} does not exist.");
            }

            var validationResult = _productUpdateRequestValidator.Validate(productUpdateRequest);

            if(!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var updatedProduct = _mapper.Map<Product>(productUpdateRequest);

            await _productsRepository.UpdateProduct(updatedProduct);

            var updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct);

            return updatedProductResponse;
        }
    }
}
