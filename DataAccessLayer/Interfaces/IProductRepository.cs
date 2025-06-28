using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IProductRepository
    {
        // Haalt alle producten op uit de database en geeft ze terug als een lijst
        public IEnumerable<Product> GetAllProducts();

        // Zoekt een specifiek product op basis van het ID, geeft null terug als niet gevonden
        public Product? GetProductById(int id);

        // Voegt een nieuw product toe aan de database
        public void AddProduct(Product product);

        // Werkt de gegevens van een bestaand product bij in de database
        public void UpdateProduct(Product product);

        // Verwijdert een product uit de database
        public void DeleteProduct(Product product);
    }
}
