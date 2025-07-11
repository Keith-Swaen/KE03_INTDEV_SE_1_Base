﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        
        // Navigatie property naar de klant die bij deze bestelling hoort
        public Customer Customer { get; set; } = null!;

        // Lijst van alle bestelde items in deze bestelling
        public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

        // AVG Compliance velden
        // Datum tot wanneer de bestellingsgegevens bewaard mogen worden volgens AVG
        public DateTime? DataRetentionUntil { get; set; }
        
        // Datum waarop is verzocht om verwijdering van deze bestellingsgegevens
        public DateTime? DataDeletionRequested { get; set; }
    }
}
