using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    // Database context klasse die de verbinding met de SQLite database beheert
    public class MatrixIncDbContext : DbContext
    {
        // Constructor die de database opties ontvangt (connection string, etc.)
        public MatrixIncDbContext(DbContextOptions<MatrixIncDbContext> options) : base(options)
        {
        }

        // Database tabellen die overeenkomen met de model classes
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Configureert de relaties tussen de verschillende entiteiten in de database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relatie: Een klant kan meerdere bestellingen hebben, elke bestelling hoort bij één klant
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired();

            // Relatie: Een bestelling kan meerdere bestelde items hebben, elk item hoort bij één bestelling
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .IsRequired();

            // Relatie: Een besteld item hoort bij één product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .IsRequired();

            // Relatie: Een onderdeel kan in meerdere producten zitten, een product kan meerdere onderdelen hebben
            modelBuilder.Entity<Part>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Parts);

            base.OnModelCreating(modelBuilder);
        }
    }
}
