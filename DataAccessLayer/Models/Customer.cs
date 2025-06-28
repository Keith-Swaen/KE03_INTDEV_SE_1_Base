using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Customer
    {
        // Unieke identifier voor de klant in de database
        [Key]
        [Required]
        public int Id { get; set; }

        // Naam van de klant, verplicht veld
        [Required]
        public string Name { get; set; }

        // Adres van de klant, verplicht veld
        [Required]
        public string Address { get; set; }

        // Geeft aan of de klant actief is in het systeem
        public bool Active { get; set; }

        // AVG Compliance velden
        // Datum waarop de klant toestemming heeft gegeven voor gegevensverwerking
        public DateTime? ConsentDate { get; set; }
        
        // Type toestemming dat de klant heeft gegeven 
        public string? ConsentType { get; set; } // "Marketing", "Necessary"
        
        // Datum tot wanneer de klantgegevens bewaard mogen worden volgens AVG
        public DateTime? DataRetentionUntil { get; set; }
        
        // Datum waarop de klant heeft verzocht om gegevensverwijdering
        public DateTime? DataDeletionRequested { get; set; }
        
        // Geeft aan of de klant zijn toestemming heeft ingetrokken
        public bool ConsentWithdrawn { get; set; }

        // Lijst van alle bestellingen die bij deze klant horen
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}