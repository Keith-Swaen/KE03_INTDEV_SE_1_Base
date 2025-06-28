using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        // Database context voor toegang tot de producten tabel
        private readonly MatrixIncDbContext _context;

        // Constructor die de database context injecteert
        public ProductRepository(MatrixIncDbContext context) 
        {
            _context = context;
        }
        
        // Voegt een nieuw product toe aan de database
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Verwijdert een product uit de database
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        // Haalt alle producten op uit de database inclusief hun onderdelen
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Parts);
        }

        // Zoekt een specifiek product op basis van ID inclusief onderdelen
        public Product? GetProductById(int id)
        {
            return _context.Products.Include(p => p.Parts).FirstOrDefault(p => p.Id == id);
        }

        // Werkt de gegevens van een bestaand product bij in de database
        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
    }
}
