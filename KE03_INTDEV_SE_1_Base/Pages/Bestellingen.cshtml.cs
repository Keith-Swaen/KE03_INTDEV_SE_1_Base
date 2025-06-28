using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    // Beheert de logica voor het tonen van alle bestellingen op de bestellingen pagina
    public class BestellingenModel : PageModel
    {
        // Database toegang voor bestellingsgegevens
        private readonly IOrderRepository _orderRepository;
        
        // Voor het opslaan van acties en fouten
        private readonly ILogger<BestellingenModel> _logger;

        // Lijst van alle bestellingen die op de pagina worden getoond
        public IList<Order> Bestellingen { get; set; } = new List<Order>();

        // Constructor die de benodigde onderdelen ontvangt
        public BestellingenModel(IOrderRepository orderRepository, ILogger<BestellingenModel> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        // Wordt aangeroepen wanneer de pagina wordt geladen
        public void OnGet()
        {
            // Haalt alle bestellingen op uit de database
            Bestellingen = _orderRepository.GetAllOrders().ToList();
            // Slaat op dat de bestellingsgeschiedenis is bekeken
            _logger.LogInformation("Order history accessed. Total orders: {OrderCount}", Bestellingen.Count);
        }
    }
}
