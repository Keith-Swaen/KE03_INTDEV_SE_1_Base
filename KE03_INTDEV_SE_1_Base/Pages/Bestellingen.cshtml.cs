using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellingenModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<BestellingenModel> _logger;

        public IList<Order> Bestellingen { get; set; } = new List<Order>();

        public BestellingenModel(IOrderRepository orderRepository, ILogger<BestellingenModel> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public void OnGet()
        {
            Bestellingen = _orderRepository.GetAllOrders().ToList();
            _logger.LogInformation("Order history accessed. Total orders: {OrderCount}", Bestellingen.Count);
        }
    }
}
