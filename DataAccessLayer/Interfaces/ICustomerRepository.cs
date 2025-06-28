using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ICustomerRepository
    {
        // Haalt alle klanten op uit de database en geeft ze terug als een lijst
        public IEnumerable<Customer> GetAllCustomers();

        // Zoekt een specifieke klant op basis van het ID, geeft null terug als niet gevonden
        public Customer? GetCustomerById(int id);

        // Voegt een nieuwe klant toe aan de database
        public void AddCustomer(Customer customer);

        // Werkt de gegevens van een bestaande klant bij in de database
        public void UpdateCustomer(Customer customer);

        // Verwijdert een klant uit de database
        public void DeleteCustomer(Customer customer);

        // AVG Compliance methoden
        // Werkt de toestemming van een klant bij voor een specifiek type 
        void UpdateConsent(int customerId, string consentType, bool consentGiven);
        
        // Markeert een klant voor gegevensverwijdering volgens AVG verzoek
        void RequestDataDeletion(int customerId);
        
        // Controleert en verwerkt klanten waarvan de gegevens bewaartermijn is verstreken
        void ProcessDataRetention();
        
        // Haalt alle klanten op waarvan de gegevens bewaartermijn is verstreken voor verwerking
        IEnumerable<Customer> GetCustomersForDataRetention();
    }
}
