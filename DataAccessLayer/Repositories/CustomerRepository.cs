using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        // Database context voor toegang tot de klanten tabel
        private readonly MatrixIncDbContext _context;
        
        // Logger voor het bijhouden van acties en fouten
        private readonly ILogger<CustomerRepository> _logger;

        // Constructor die de database context en logger injecteert
        public CustomerRepository(MatrixIncDbContext context, ILogger<CustomerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Voegt een nieuwe klant toe aan de database
        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        // Verwijdert een klant uit de database
        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        // Haalt alle klanten op uit de database inclusief hun bestellingen
        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.Customers.Include(c => c.Orders);
        }

        // Zoekt een specifieke klant op basis van ID inclusief bestellingen
        public Customer? GetCustomerById(int id)
        {
            return _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.Id == id);
        }

        // Werkt de gegevens van een bestaande klant bij in de database
        public void UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        // AVG Compliance methoden
        // Werkt de toestemming van een klant bij voor gegevensverwerking
        public void UpdateConsent(int customerId, string consentType, bool consentGiven)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                if (consentGiven)
                {
                    // Als toestemming wordt gegeven, zet de datum en type
                    customer.ConsentDate = DateTime.Now;
                    customer.ConsentType = consentType;
                    customer.ConsentWithdrawn = false;
                    // Zet bewaartermijn op 7 jaar voor bestellingen
                    customer.DataRetentionUntil = DateTime.Now.AddYears(7);
                    _logger.LogInformation("Klant {CustomerId} heeft toestemming gegeven voor {ConsentType}", customerId, consentType);
                }
                else
                {
                    // Als toestemming wordt ingetrokken, markeer voor verwijdering
                    customer.ConsentWithdrawn = true;
                    customer.DataDeletionRequested = DateTime.Now;
                    _logger.LogInformation("Klant {CustomerId} heeft toestemming ingetrokken voor {ConsentType}", customerId, consentType);
                }
                
                _context.SaveChanges();
            }
        }

        // Markeert een klant voor gegevensverwijdering volgens AVG verzoek
        public void RequestDataDeletion(int customerId)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                customer.DataDeletionRequested = DateTime.Now;
                customer.ConsentWithdrawn = true;
                _logger.LogInformation("Gegevensverwijdering aangevraagd voor klant {CustomerId}", customerId);
                _context.SaveChanges();
            }
        }

        // Controleert en verwerkt klanten waarvan de bewaartermijn is verstreken
        public void ProcessDataRetention()
        {
            var customersForDeletion = GetCustomersForDataRetention();
            foreach (var customer in customersForDeletion)
            {
                _logger.LogInformation("Bewaartermijn verwerking voor klant {CustomerId}. Bewaartermijn verlopen.", customer.Id);
                
                // Anonimiseer klantgegevens in plaats van volledige verwijdering voor wettelijke compliance
                customer.Name = "GEANONYMISEERD";
                customer.Address = "GEANONYMISEERD";
                customer.Active = false;
                customer.ConsentWithdrawn = true;
                
                // Markeer bestellingen voor verwijdering na extra periode
                foreach (var order in customer.Orders)
                {
                    order.DataDeletionRequested = DateTime.Now;
                }
            }
            
            if (customersForDeletion.Any())
            {
                _context.SaveChanges();
                _logger.LogInformation("Bewaartermijn verwerkt voor {Count} klanten", customersForDeletion.Count());
            }
        }

        // Haalt alle klanten op waarvan de bewaartermijn is verstreken
        public IEnumerable<Customer> GetCustomersForDataRetention()
        {
            return _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.DataRetentionUntil.HasValue && 
                           c.DataRetentionUntil.Value < DateTime.Now &&
                           !c.DataDeletionRequested.HasValue)
                .ToList();
        }
    }
}
