using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductsModel> _logger;

        public ProductsModel(IProductRepository productRepository, ILogger<ProductsModel> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PriceRange { get; set; }

        public void OnGet()
        {
            var products = _productRepository.GetAllProducts();
            var initialCount = products.Count();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                products = products
                    .Where(p => p.Name.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
                _logger.LogInformation("Product search performed with term '{SearchTerm}'. Results: {FilteredCount}/{TotalCount}", 
                    SearchTerm, products.Count(), initialCount);
            }

            if (!string.IsNullOrWhiteSpace(PriceRange))
            {
                products = PriceRange switch
                {
                    "low" => products.Where(p => p.Price < 250).ToList(),
                    "mid" => products.Where(p => p.Price >= 250 && p.Price <= 550).ToList(),
                    "high" => products.Where(p => p.Price > 550).ToList(),
                    _ => products
                };
                _logger.LogInformation("Price filter applied: '{PriceRange}'. Results: {FilteredCount}", 
                    PriceRange, products.Count());
            }

            Products = products;
        }

        public IActionResult OnPostAddToCart(int productId, int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null || quantity < 1)
            {
                _logger.LogWarning("Failed to add product to cart. ProductId: {ProductId}, Quantity: {Quantity}. Product not found or invalid quantity.", 
                    productId, quantity);
                return BadRequest();
            }

            var sessionData = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(sessionData)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(sessionData);

            var existingItem = cart!.FirstOrDefault(p => p.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _logger.LogInformation("Updated cart item quantity. Product: '{ProductName}', New Quantity: {Quantity}", 
                    product.Name, existingItem.Quantity);
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity });
                _logger.LogInformation("Added new item to cart. Product: '{ProductName}', Quantity: {Quantity}", 
                    product.Name, quantity);
            }

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return new JsonResult(new { success = true });
        }

        public class CartItem
        {
            public Product Product { get; set; } = new();
            public int Quantity { get; set; }
        }
    }
}
