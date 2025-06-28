namespace DataAccessLayer.Models
{
    public class OrderItem
    {
        // Unieke identifier voor het bestelde item in de database
        public int Id { get; set; }

        // ID van de bestelling waar dit item bij hoort
        public int OrderId { get; set; }
        
        // Navigatie property naar de bestelling die bij dit item hoort
        public Order Order { get; set; } = null!;

        // ID van het product dat is besteld
        public int ProductId { get; set; }
        
        // Navigatie property naar het product dat bij dit item hoort
        public Product Product { get; set; } = null!;

        // Aantal van dit product dat is besteld
        public int Quantity { get; set; }
    }
}
