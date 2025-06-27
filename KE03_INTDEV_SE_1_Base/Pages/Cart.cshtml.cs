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
    public class CartModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CartModel> _logger;

        public List<CartItem> CartItems { get; set; } = new();

        public CartModel(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<CartModel> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public void OnGet()
        {
            LoadCart();
            _logger.LogInformation("Cart page loaded. Items in cart: {ItemCount}", CartItems.Count);
        }

        private void LoadCart()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(sessionData))
            {
                CartItems = JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
            }
        }

        private void SaveCart()
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(CartItems));
        }

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

        public IActionResult OnPostCheckout()
        {
            LoadCart();

            if (!CartItems.Any())
            {
                _logger.LogWarning("Checkout attempted with empty cart");
                TempData["Error"] = "Je winkelmand is leeg, er kon niet besteld worden.";
                return RedirectToPage();
            }

            _logger.LogInformation("Starting checkout process. Items in cart: {ItemCount}, Total items: {TotalItems}", 
                CartItems.Count, CartItems.Sum(i => i.Quantity));

            var nieuweOrder = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = 1
            };

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
                _orderRepository.AddOrder(nieuweOrder);
                var totalAmount = nieuweOrder.OrderItems.Sum(oi => oi.Product.Price * oi.Quantity);
                _logger.LogInformation("Order successfully placed. OrderId: {OrderId}, Total Amount: {TotalAmount}, Items: {ItemCount}", 
                    nieuweOrder.Id, totalAmount, nieuweOrder.OrderItems.Count);
                
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

    public class CartItem
    {
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
