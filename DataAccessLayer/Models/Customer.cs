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
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public bool Active { get; set; }

        // AVG Compliance velden
        public DateTime? ConsentDate { get; set; }
        public string? ConsentType { get; set; } // "Marketing", "Necessary"
        public DateTime? DataRetentionUntil { get; set; }
        public DateTime? DataDeletionRequested { get; set; }
        public bool ConsentWithdrawn { get; set; }

        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}