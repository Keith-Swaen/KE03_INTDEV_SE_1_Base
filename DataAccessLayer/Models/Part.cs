using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Part
    {
        // Unieke identifier voor het onderdeel in de database
        public int Id { get; set; }

        // Naam van het onderdeel
        public string Name { get; set; }

        // Beschrijving van het onderdeel
        public string Description { get; set; }

        // Lijst van alle producten die dit onderdeel gebruiken
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
