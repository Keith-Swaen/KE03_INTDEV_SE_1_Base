using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IOrderRepository
    {
        // Haalt alle bestellingen op uit de database en geeft ze terug als een lijst
        public IEnumerable<Order> GetAllOrders();

        // Zoekt een specifieke bestelling op basis van het ID, geeft null terug als niet gevonden
        public Order? GetOrderById(int id);

        public void AddOrder(Order order);

        public void UpdateOrder(Order order);

        public void DeleteOrder(Order order);
    }
}
