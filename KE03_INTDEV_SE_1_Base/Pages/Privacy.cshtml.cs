using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer.Interfaces;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly ICustomerRepository _customerRepository;

        public PrivacyModel(ILogger<PrivacyModel> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public string ConsentType { get; set; } = "Necessary";

        [BindProperty]
        public bool ConsentGiven { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Privacybeleid pagina bezocht");
        }

        public IActionResult OnPostGiveConsent()
        {
            if (ConsentGiven)
            {
                _customerRepository.UpdateConsent(1, ConsentType, true); // Hardcoded klant ID voor demo
                _logger.LogInformation("AVG toestemming gegeven door klant 1 voor type: {ConsentType}", ConsentType);
                TempData["Message"] = "Bedankt voor je toestemming. Je kunt nu bestellingen plaatsen.";
            }
            else
            {
                _customerRepository.UpdateConsent(1, ConsentType, false);
                _logger.LogInformation("AVG toestemming ingetrokken door klant 1 voor type: {ConsentType}", ConsentType);
                TempData["Message"] = "Je toestemming is ingetrokken. Je kunt geen bestellingen meer plaatsen.";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRequestDeletion()
        {
            _customerRepository.RequestDataDeletion(1); // Hardcoded klant ID voor demo
            _logger.LogInformation("Gegevensverwijdering aangevraagd door klant 1");
            TempData["Message"] = "Je verzoek tot gegevensverwijdering is ontvangen. We verwerken dit binnen 30 dagen.";
            
            return RedirectToPage();
        }
    }
}
