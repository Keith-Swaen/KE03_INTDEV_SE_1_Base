using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Product
    {        
        // Unieke identifier voor het product in de database
        public int Id { get; set; }

        // Naam van het product
        public string Name { get; set; }

        // Beschrijving van het product
        public string Description { get; set; }

        // Prijs van het product in euro's
        public decimal Price { get; set; }

        // Lijst van alle onderdelen die in dit product zitten
        public ICollection<Part> Parts { get; } = new List<Part>();
    }
}
