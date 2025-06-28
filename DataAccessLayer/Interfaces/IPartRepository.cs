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

        public void AddPart(Part part);

        public void UpdatePart(Part part);

        public void DeletePart(Part part);
    }
}
