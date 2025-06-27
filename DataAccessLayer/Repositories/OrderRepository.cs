using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MatrixIncDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(MatrixIncDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            _logger.LogInformation("Retrieving all orders from database");
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();
            _logger.LogInformation("Retrieved {OrderCount} orders from database", orders.Count);
            return orders;
        }

        public Order? GetOrderById(int id)
        {
            _logger.LogInformation("Retrieving order by ID: {OrderId}", id);
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);
            
            if (order != null)
            {
                _logger.LogInformation("Order found. OrderId: {OrderId}, Customer: {CustomerName}, Items: {ItemCount}", 
                    order.Id, order.Customer.Name, order.OrderItems.Count);
            }
            else
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}", id);
            }
            
            return order;
        }

        public void AddOrder(Order order)
        {
            _logger.LogInformation("Adding new order to database. CustomerId: {CustomerId}, Items: {ItemCount}", 
                order.CustomerId, order.OrderItems.Count);
            
            _context.Orders.Add(order);
            _context.SaveChanges();
            
            _logger.LogInformation("Order successfully added to database. OrderId: {OrderId}", order.Id);
        }

        public void UpdateOrder(Order order)
        {
            _logger.LogInformation("Updating order in database. OrderId: {OrderId}", order.Id);
            _context.Orders.Update(order);
            _context.SaveChanges();
            _logger.LogInformation("Order successfully updated. OrderId: {OrderId}", order.Id);
        }

        public void DeleteOrder(Order order)
        {
            _logger.LogInformation("Deleting order from database. OrderId: {OrderId}", order.Id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            _logger.LogInformation("Order successfully deleted. OrderId: {OrderId}", order.Id);
        }
    }
}
