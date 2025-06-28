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

        public void AddProduct(Product product);

        public void UpdateProduct(Product product);

        public void DeleteProduct(Product product);
    }
}
