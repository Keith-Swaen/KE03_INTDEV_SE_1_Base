using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class CartModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public List<CartItem> CartItems { get; set; } = new();

        public CartModel(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            LoadCart();
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
                CartItems.Remove(item);
                SaveCart();
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
                item.Quantity = newQuantity;
                SaveCart();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostCheckout()
        {
            LoadCart();

            if (!CartItems.Any())
            {
                TempData["Error"] = "Je winkelmand is leeg, er kon niet besteld worden.";
                return RedirectToPage();
            }

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
                }
            }

            try
            {
                _orderRepository.AddOrder(nieuweOrder);
                CartItems.Clear();
                SaveCart();

                TempData["Message"] = "Bestelling geplaatst!";
            }
            catch
            {
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
