using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    // Beheert de winkelwagen functionaliteit inclusief toevoegen, bijwerken, verwijderen en bestellen
    public class CartModel : PageModel
    {
        // Database toegang voor verschillende gegevens
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CartModel> _logger;
        private readonly ICustomerRepository _customerRepository;

        // Lijst van items in de winkelwagen
        public List<CartItem> CartItems { get; set; } = new();

        // Constructor die de benodigde onderdelen ontvangt
        public CartModel(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<CartModel> logger, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
            _customerRepository = customerRepository;
        }

        // Wordt aangeroepen wanneer de winkelwagen pagina wordt geladen
        public void OnGet()
        {
            LoadCart();
            _logger.LogInformation("Cart page loaded. Items in cart: {ItemCount}", CartItems.Count);
        }

        // Laadt de winkelwagen gegevens uit het geheugen
        private void LoadCart()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(sessionData))
            {
                CartItems = JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
            }
        }

        // Slaat de winkelwagen gegevens op in het geheugen
        private void SaveCart()
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(CartItems));
        }

        // Verwijdert een product uit de winkelwagen
        public IActionResult OnPostRemoveItem(int productId)
        {
            LoadCart();
            var item = CartItems.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                _logger.LogInformation("Removing item from cart. Product: '{ProductName}', Quantity: {Quantity}", 
                    item.Product.Name, item.Quantity);
                CartItems.Remove(item);
                SaveCart();
            }
            else
            {
                _logger.LogWarning("Attempted to remove non-existent item from cart. ProductId: {ProductId}", productId);
            }
            return RedirectToPage();
        }

        // Werkt de hoeveelheid van een product in de winkelwagen bij
        public IActionResult OnPostUpdateQuantity(int productId, int newQuantity)
        {
            LoadCart();
            if (newQuantity < 1) newQuantity = 1;

            var item = CartItems.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                var oldQuantity = item.Quantity;
                item.Quantity = newQuantity;
                _logger.LogInformation("Updated cart item quantity. Product: '{ProductName}', Old Quantity: {OldQuantity}, New Quantity: {NewQuantity}", 
                    item.Product.Name, oldQuantity, newQuantity);
                SaveCart();
            }
            else
            {
                _logger.LogWarning("Attempted to update quantity for non-existent item. ProductId: {ProductId}, NewQuantity: {NewQuantity}", 
                    productId, newQuantity);
            }
            return RedirectToPage();
        }

        // Verwerkt de bestelling en maakt een nieuwe order aan
        public IActionResult OnPostCheckout()
        {
            LoadCart();

            // Controleert of de winkelwagen niet leeg is
            if (!CartItems.Any())
            {
                _logger.LogWarning("Checkout attempted with empty cart");
                TempData["Error"] = "Je winkelmand is leeg, er kon niet besteld worden.";
                return RedirectToPage();
            }

            // Controleert of klant toestemming heeft gegeven voor gegevensverwerking (AVG wet)
            var customer = _customerRepository.GetCustomerById(1); // Hardcoded voor demo
            if (customer == null || customer.ConsentWithdrawn)
            {
                _logger.LogWarning("Checkout geprobeerd zonder geldige AVG toestemming. KlantId: 1");
                TempData["Error"] = "Je moet eerst toestemming geven voor het verwerken van je gegevens. Bekijk onze privacyverklaring.";
                return RedirectToPage();
            }

            _logger.LogInformation("Starting checkout process. Items in cart: {ItemCount}, Total items: {TotalItems}", 
                CartItems.Count, CartItems.Sum(i => i.Quantity));

            // Maakt een nieuwe bestelling aan
            var nieuweOrder = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = 1,
                DataRetentionUntil = DateTime.Now.AddYears(7) // 7 jaar bewaartermijn volgens AVG
            };

            // Voegt alle items uit de winkelwagen toe aan de bestelling
            foreach (var item in CartItems)
            {
                var productFromDb = _productRepository.GetProductById(item.Product.Id);
                if (productFromDb != null)
                {
                    nieuweOrder.OrderItems.Add(new OrderItem
                    {
                        Product = productFromDb,
                        ProductId = productFromDb.Id,
                        Quantity = item.Quantity
                    });
                    _logger.LogInformation("Added item to order. Product: '{ProductName}', Quantity: {Quantity}, Price: {Price}", 
                        productFromDb.Name, item.Quantity, productFromDb.Price);
                }
                else
                {
                    _logger.LogError("Product not found during checkout. ProductId: {ProductId}", item.Product.Id);
                }
            }

            try
            {
                // Slaat de bestelling op in de database
                _orderRepository.AddOrder(nieuweOrder);
                var totalAmount = nieuweOrder.OrderItems.Sum(oi => oi.Product.Price * oi.Quantity);
                _logger.LogInformation("Order successfully placed. OrderId: {OrderId}, Total Amount: {TotalAmount}, Items: {ItemCount}", 
                    nieuweOrder.Id, totalAmount, nieuweOrder.OrderItems.Count);
                
                // Leegt de winkelwagen na succesvolle bestelling
                CartItems.Clear();
                SaveCart();

                TempData["Message"] = "Bestelling geplaatst!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place order. Error: {ErrorMessage}", ex.Message);
                TempData["Error"] = "Er is iets misgegaan bij het plaatsen van de bestelling.";
            }

            return RedirectToPage();
        }
    }

    // Model klasse voor items in de winkelwagen
    public class CartItem
    {
        // Het product dat in de winkelwagen zit
        public Product Product { get; set; } = default!;
        
        // Aantal van dit product
        public int Quantity { get; set; }
    }
}
