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

        public ProductsModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PriceRange { get; set; }

        public void OnGet()
        {
            var products = _productRepository.GetAllProducts();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                products = products
                    .Where(p => p.Name.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
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
            }

            Products = products;
        }

        public IActionResult OnPostAddToCart(int productId, int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null || quantity < 1)
            {
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
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity });
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
