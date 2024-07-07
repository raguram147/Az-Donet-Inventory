using InventoryManagement.Interfaces;
using InventoryManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository,IMemoryCache cache, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _cache = cache;
            _logger = logger;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            if (!_cache.TryGetValue(id, out Product product))
            {
                product = await _productRepository.GetByIdAsync(id);
                if (product != null)
                {
                    _cache.Set(id, product, TimeSpan.FromMinutes(5));  // Cache for n minutes
                    _logger.LogInformation($"Product with ID {id} fetched from repository.");
                }
                else
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                }
            }
            else
            {
                _logger.LogInformation($"Product with ID {id} fetched from cache.");
            }
            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
