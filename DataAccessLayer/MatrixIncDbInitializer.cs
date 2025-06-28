using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Statische klasse die de database vult met initiële testgegevens
    public static class MatrixIncDbInitializer
    {
        // Initialiseert de database met dummy data als deze nog leeg is
        public static void Initialize(MatrixIncDbContext context)
        {
            // Controleert of er al klanten in de database staan
            if (context.Customers.Any())
            {
                return;   // Database is al gevuld met testgegevens
            }

            // TODO: Hier moet ik nog wat namen verzinnen die betrekking hebben op de matrix.
            // - Denk aan de m3 boutjes, moertjes en ringetjes.
            // - Denk aan namen van schepen
            // - Denk aan namen van vliegtuigen            
            
            // Maakt dummy klanten aan 
            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St" , Active=true},
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            context.Customers.AddRange(customers);

            // Maakt dummy producten aan 
            var products = new Product[]
            {
                new Product { Name = "Nebuchadnezzar", Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen", Price = 10000.00m },
                new Product { Name = "Jack-in Chair", Description = "Stoel met een rugsteun en metalen armen waarin mensen zitten om ingeplugd te worden in de Matrix via een kabel in de nekpoort", Price = 500.50m },
                new Product { Name = "EMP (Electro-Magnetic Pulse) Device", Description = "Wapentuig op de schepen van Zion", Price = 129.99m }
            };
            context.Products.AddRange(products);

            // Maakt dummy bestellingen aan voor de verschillende klanten
            var orders = new Order[]
            {
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-01-01")},
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-02-01")},
                new Order { Customer = customers[1], OrderDate = DateTime.Parse("2021-02-01")},
                new Order { Customer = customers[2], OrderDate = DateTime.Parse("2021-03-01")}
            };  
            context.Orders.AddRange(orders);

            // Voegt bestelde items toe aan de dummy bestellingen met verschillende hoeveelheden
            var orderItems = new OrderItem[]
            {
                new OrderItem { Order = orders[0], Product = products[0], ProductId = products[0].Id, Quantity = 1 },
                new OrderItem { Order = orders[0], Product = products[1], ProductId = products[1].Id, Quantity = 2 },
                new OrderItem { Order = orders[1], Product = products[2], ProductId = products[2].Id, Quantity = 3 },
                new OrderItem { Order = orders[2], Product = products[0], ProductId = products[0].Id, Quantity = 1 },
                new OrderItem { Order = orders[3], Product = products[1], ProductId = products[1].Id, Quantity = 1 },
                new OrderItem { Order = orders[3], Product = products[2], ProductId = products[2].Id, Quantity = 2 }
            };
            context.OrderItems.AddRange(orderItems);

            // Maakt dummy onderdelen aan die in producten kunnen zitten
            var parts = new Part[]
            {
                new Part { Name = "Tandwiel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen"},
                new Part { Name = "M5 Boutje", Description = "Bevestiging van panelen, buizen of interne modules"},
                new Part { Name = "Hydraulische cilinder", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen"},
                new Part { Name = "Koelvloeistofpomp", Description = "Koeling van de motor of elektronische systemen."}
            };
            context.Parts.AddRange(parts);

            // Slaat alle wijzigingen op in de database
            context.SaveChanges();

            // Zorgt ervoor dat de database wordt aangemaakt als deze nog niet bestaat
            context.Database.EnsureCreated();
        }
    }
}
