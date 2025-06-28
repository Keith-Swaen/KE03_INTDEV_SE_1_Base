using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer.Interfaces;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    // Beheert de privacy pagina en AVG compliance functionaliteit zoals toestemming en gegevensverwijdering
    public class PrivacyModel : PageModel
    {
        // Voor het opslaan van acties en fouten
        private readonly ILogger<PrivacyModel> _logger;
        
        // Database toegang voor klantgegevens voor AVG compliance
        private readonly ICustomerRepository _customerRepository;

        // Constructor die de benodigde onderdelen ontvangt
        public PrivacyModel(ILogger<PrivacyModel> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        // Type toestemming dat de gebruiker kan geven, gebonden aan formulier
        [BindProperty]
        public string ConsentType { get; set; } = "Necessary";

        // Of de gebruiker toestemming heeft gegeven, gebonden aan formulier
        [BindProperty]
        public bool ConsentGiven { get; set; }

        // Wordt aangeroepen wanneer de privacy pagina wordt geladen
        public void OnGet()
        {
            _logger.LogInformation("Privacybeleid pagina bezocht");
        }

        // Verwerkt het geven of intrekken van AVG toestemming
        public IActionResult OnPostGiveConsent()
        {
            if (ConsentGiven)
            {
                // Werkt de toestemming bij in de database
                _customerRepository.UpdateConsent(1, ConsentType, true); // Hardcoded klant ID voor demo
                _logger.LogInformation("AVG toestemming gegeven door klant 1 voor type: {ConsentType}", ConsentType);
                TempData["Message"] = "Bedankt voor je toestemming. Je kunt nu bestellingen plaatsen.";
            }
            else
            {
                // Trekt de toestemming in
                _customerRepository.UpdateConsent(1, ConsentType, false);
                _logger.LogInformation("AVG toestemming ingetrokken door klant 1 voor type: {ConsentType}", ConsentType);
                TempData["Message"] = "Je toestemming is ingetrokken. Je kunt geen bestellingen meer plaatsen.";
            }

            return RedirectToPage();
        }

        // Verwerkt een verzoek tot gegevensverwijdering volgens AVG
        public IActionResult OnPostRequestDeletion()
        {
            // Markeert de klant voor gegevensverwijdering
            _customerRepository.RequestDataDeletion(1); // Hardcoded klant ID voor demo
            _logger.LogInformation("Gegevensverwijdering aangevraagd door klant 1");
            TempData["Message"] = "Je verzoek tot gegevensverwijdering is ontvangen. We verwerken dit binnen 30 dagen.";
            
            return RedirectToPage();
        }
    }
}
