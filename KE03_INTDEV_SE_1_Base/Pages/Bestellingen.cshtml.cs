using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellingenModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;

        public IList<Order> Bestellingen { get; set; } = new List<Order>();

        public BestellingenModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void OnGet()
        {
            Bestellingen = _orderRepository.GetAllOrders().ToList();
        }
    }
}
