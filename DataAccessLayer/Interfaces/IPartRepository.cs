using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPartRepository
    {
        // Haalt alle onderdelen op uit de database en geeft ze terug als een lijst
        public IEnumerable<Part> GetAllParts();

        // Zoekt een specifiek onderdeel op basis van het ID, geeft null terug als niet gevonden
        public Part? GetPartById(int id);

        // Voegt een nieuw onderdeel toe aan de database
        public void AddPart(Part part);

        // Werkt de gegevens van een bestaand onderdeel bij in de database
        public void UpdatePart(Part part);

        // Verwijdert een onderdeel uit de database
        public void DeletePart(Part part);
    }
}
