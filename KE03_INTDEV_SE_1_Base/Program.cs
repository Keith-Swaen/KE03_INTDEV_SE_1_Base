using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_1_Base
{
    public class Program
    {
        // Startpunt van de applicatie
        public static void Main(string[] args)
        {
            // Maakt een nieuwe web applicatie builder aan
            var builder = WebApplication.CreateBuilder(args);

            // Voegt de database context toe met SQLite database
            builder.Services.AddDbContext<MatrixIncDbContext>(
                options => options.UseSqlite("Data Source=MatrixInc.db"));

            // Registreert alle repository interfaces en implementaties voor dependency injection
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();
            builder.Services.AddScoped<ProductRepository>();

            // Voegt Razor Pages toe voor de webpagina's
            builder.Services.AddRazorPages();

            // Nodig voor het gebruik van sessieopslag in het geheugen
            builder.Services.AddDistributedMemoryCache();

            // Sessie functionaliteit inschakelen
            builder.Services.AddSession();

            // Bouwt de web applicatie
            var app = builder.Build();

            // Configureert error handling voor productie omgeving
            if (!app.Environment.IsDevelopment())
            {
                // Toont een foutpagina in plaats van technische details
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Initialiseert de database en vult deze met testgegevens
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Haalt de database context op
                var context = services.GetRequiredService<MatrixIncDbContext>();
                // Zorgt ervoor dat de database wordt aangemaakt als deze nog niet bestaat
                context.Database.EnsureCreated();
                // Vult de database met dummy data (klanten, producten, bestellingen)
                MatrixIncDbInitializer.Initialize(context);
            }

            // Stuurt HTTP verzoeken door naar HTTPS voor veiligheid
            app.UseHttpsRedirection();
            // Maakt statische bestanden beschikbaar bijvoorbeeld CSS, JavaScript, afbeeldingen
            app.UseStaticFiles();

            // Schakelt routing in voor URL verwerking
            app.UseRouting();

            // Nodig zodat sessie werkt via cookies
            app.UseCookiePolicy();

            // Activeert sessie functionaliteit voor winkelwagen en gebruikersgegevens
            app.UseSession();

            // Schakelt autorisatie in 
            app.UseAuthorization();

            // Koppelt Razor Pages aan URL routes
            app.MapRazorPages();

            // Start de web applicatie
            app.Run();
        }
    }
}
