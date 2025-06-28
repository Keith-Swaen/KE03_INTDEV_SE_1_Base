using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    // Beheert de homepage en toont een overzicht van alle klanten met hun bestellingsinformatie
    public class IndexModel : PageModel
    {
        // Voor het opslaan van acties en fouten
        private readonly ILogger<IndexModel> _logger;
        
        // Database toegang voor klantgegevens
        private readonly ICustomerRepository _customerRepository;

        // Lijst van alle klanten die op de homepage worden getoond
        public IList<Customer> Customers { get; set; }

        // Constructor die de benodigde onderdelen ontvangt
        public IndexModel(ILogger<IndexModel> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            Customers = new List<Customer>();
        }

        // Wordt aangeroepen wanneer de homepage wordt geladen
        public void OnGet()
        {            
            // Haalt alle klanten op uit de database voor het overzicht
            Customers = _customerRepository.GetAllCustomers().ToList();                            
            _logger.LogInformation($"getting all {Customers.Count} customers");
        }
    }
}
