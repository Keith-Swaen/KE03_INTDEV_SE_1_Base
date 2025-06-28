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
    // Beheert de producten pagina met zoek en filterfunctionaliteit en winkelwagen toevoeging
    public class ProductsModel : PageModel
    {
        // Database toegang voor productgegevens
        private readonly IProductRepository _productRepository;
        
        // Voor het opslaan van acties en fouten
        private readonly ILogger<ProductsModel> _logger;

        // Constructor die de benodigde onderdelen ontvangt
        public ProductsModel(IProductRepository productRepository, ILogger<ProductsModel> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        // Lijst van producten die op de pagina worden getoond, wel gefilterd
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // Zoekterm voor het filteren van producten, gebonden aan URL parameter
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Prijsfilter voor het filteren van producten, gebonden aan URL parameter
        [BindProperty(SupportsGet = true)]
        public string? PriceRange { get; set; }

        // Wordt aangeroepen wanneer de producten pagina wordt geladen
        public void OnGet()
        {
            // Haalt alle producten op uit de database
            var products = _productRepository.GetAllProducts();
            var initialCount = products.Count();

            // Filtert producten op basis van zoekterm 
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                products = products
                    .Where(p => p.Name.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
                _logger.LogInformation("Product search performed with term '{SearchTerm}'. Results: {FilteredCount}/{TotalCount}", 
                    SearchTerm, products.Count(), initialCount);
            }

            // Filtert producten op basis van prijsbereik
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

        // Voegt een product toe aan de winkelwagen via AJAX
        public IActionResult OnPostAddToCart(int productId, int quantity)
        {
            // Controleert of het product bestaat en de hoeveelheid geldig is
            var product = _productRepository.GetProductById(productId);
            if (product == null || quantity < 1)
            {
                _logger.LogWarning("Failed to add product to cart. ProductId: {ProductId}, Quantity: {Quantity}. Product not found or invalid quantity.", 
                    productId, quantity);
                return BadRequest();
            }

            // Laadt de huidige winkelwagen uit het geheugen
            var sessionData = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(sessionData)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();

            // Controleert of het product al in de winkelwagen zit
            var existingItem = cart.FirstOrDefault(p => p.Product.Id == productId);
            if (existingItem != null)
            {
                // Verhoogt de hoeveelheid als het product al in de winkelwagen zit
                existingItem.Quantity += quantity;
                _logger.LogInformation("Updated cart item quantity. Product: '{ProductName}', New Quantity: {Quantity}", 
                    product.Name, existingItem.Quantity);
            }
            else
            {
                // Voegt het product toe als het nog niet in de winkelwagen zit
                cart.Add(new CartItem { Product = product, Quantity = quantity });
                _logger.LogInformation("Added new item to cart. Product: '{ProductName}', Quantity: {Quantity}", 
                    product.Name, quantity);
            }

            // Slaat de bijgewerkte winkelwagen op in het geheugen
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return new JsonResult(new { success = true });
        }

        // Model klasse voor items in de winkelwagen
        public class CartItem
        {
            // Het product dat in de winkelwagen zit
            public Product Product { get; set; } = new();
            
            // Aantal van dit product
            public int Quantity { get; set; }
        }
    }
}
