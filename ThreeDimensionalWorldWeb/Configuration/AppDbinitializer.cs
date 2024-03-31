using System.Text.Json;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;
using ThreeDimensionalWorldWeb.Areas.Admin.Models;

namespace ThreeDimensionalWorldWeb.Configuration
{
    public class AppDbinitializer
    {
        public static void SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var environment = serviceScope.ServiceProvider.GetService<IWebHostEnvironment>();

                if (context == null)
                {
                    throw new Exception("Couldn't initialize ApplicationDbContext");
                }

                if(environment == null)
                {
                    throw new Exception("Couldn't initialize IWebHostEnvironment");
                }

                context.Database.EnsureCreated();

                if (!context.Categories.Any() || true)
                {
                    var jsonData = File.ReadAllText(Path.Combine(environment!.ContentRootPath, "SeedData", "Categories.json"));
                    var data = JsonSerializer.Deserialize<List<Category>>(jsonData);
                }

                if (!context.Products.Any() || true)
                {
                    var jsonData = File.ReadAllText(Path.Combine(environment!.ContentRootPath, "SeedData", "Products.json"));
                    var data = JsonSerializer.Deserialize<List<Product>>(jsonData);
                }

                if (!context.ProductFiles.Any() || true)
                {
                    var jsonData = File.ReadAllText(Path.Combine(environment!.ContentRootPath, "SeedData", "ProductFiles.json"));
                    var data = JsonSerializer.Deserialize<List<ProductFile>>(jsonData);
                }



            }
        }
    }
}
