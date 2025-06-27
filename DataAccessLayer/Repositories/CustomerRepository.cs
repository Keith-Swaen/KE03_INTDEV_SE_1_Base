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
        private readonly MatrixIncDbContext _context;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(MatrixIncDbContext context, ILogger<CustomerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.Customers.Include(c => c.Orders);
        }

        public Customer? GetCustomerById(int id)
        {
            return _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        // AVG Compliance methoden
        public void UpdateConsent(int customerId, string consentType, bool consentGiven)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer != null)
            {
                if (consentGiven)
                {
                    customer.ConsentDate = DateTime.Now;
                    customer.ConsentType = consentType;
                    customer.ConsentWithdrawn = false;
                    customer.DataRetentionUntil = DateTime.Now.AddYears(7); // 7 jaar bewaartermijn voor bestellingen
                    _logger.LogInformation("Klant {CustomerId} heeft toestemming gegeven voor {ConsentType}", customerId, consentType);
                }
                else
                {
                    customer.ConsentWithdrawn = true;
                    customer.DataDeletionRequested = DateTime.Now;
                    _logger.LogInformation("Klant {CustomerId} heeft toestemming ingetrokken voor {ConsentType}", customerId, consentType);
                }
                
                _context.SaveChanges();
            }
        }

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
