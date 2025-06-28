using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Part
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        // Lijst van alle producten die dit onderdeel gebruiken
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
