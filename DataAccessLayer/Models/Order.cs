using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

        // AVG Compliance velden
        public DateTime? DataRetentionUntil { get; set; }
        public DateTime? DataDeletionRequested { get; set; }
    }
}
