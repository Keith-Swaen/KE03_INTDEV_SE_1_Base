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

        // Voegt een nieuwe bestelling toe aan de database
        public void AddOrder(Order order);

        // Werkt de gegevens van een bestaande bestelling bij in de database
        public void UpdateOrder(Order order);

        // Verwijdert een bestelling uit de database
        public void DeleteOrder(Order order);
    }
}
