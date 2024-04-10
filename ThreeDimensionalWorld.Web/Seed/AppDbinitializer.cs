using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;
using ThreeDimensionalWorld.Web.Areas.Admin.Models;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;
using ThreeDimensionalWorld.Web.Seed.Models;

namespace ThreeDimensionalWorld.Web.Seed
{
    public class AppDbinitializer
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var environment = serviceScope.ServiceProvider.GetService<IWebHostEnvironment>();

                if (context == null)
                {
                    throw new Exception("Couldn't initialize ApplicationDbContext");
                }

                if (environment == null)
                {
                    throw new Exception("Couldn't initialize IWebHostEnvironment");
                }

                context.Database.EnsureCreated();

                string pathToData = Path.Combine(Path.Combine(environment!.ContentRootPath, "Seed", "JSONData"));
                string pathToFiles = Path.Combine(Path.Combine(environment!.ContentRootPath, "Seed", "Files"));
                string pathToImages = Path.Combine(Path.Combine(environment!.WebRootPath, "images"));
                string pathTo3dModels = Path.Combine(Path.Combine(environment!.WebRootPath, "3dModels"));


                if (!context.Categories.Any())
                {
                    var jsonData = File.ReadAllText(Path.Combine(pathToData, "Categories.json"));
                    List<Category> data = JsonSerializer.Deserialize<List<Category>>(jsonData)!;

                    foreach (var item in data)
                    {
                        item.Image = FileManager.CopyFile(Path.Combine(pathToFiles, item.Image), pathToImages);
                        context.Add(item);
                        context.SaveChanges();
                    }

                }

                if (!context.Products.Any())
                {
                    var jsonData = File.ReadAllText(Path.Combine(pathToData, "Products.json"));
                    List<Product> data = JsonSerializer.Deserialize<List<Product>>(jsonData)!;

                    foreach(var item in data)
                    {
                        context.Add(item);
                        context.SaveChanges();
                    }
                }

                if (!context.ProductFiles.Any())
                {
                    var jsonData = File.ReadAllText(Path.Combine(pathToData, "ProductFiles.json"));
                    List<ProductFile> data = JsonSerializer.Deserialize<List<ProductFile>>(jsonData)!;

                    foreach (var item in data)
                    {
                        if (AllowedFormats.Allowed3dFormats.Contains(Path.GetExtension(item.Name)))
                        {
                            item.Name = FileManager.CopyFile(Path.Combine(pathToFiles, item.Name), pathTo3dModels);
                        }
                        else
                        {
                            item.Name = FileManager.CopyFile(Path.Combine(pathToFiles, item.Name), pathToImages);
                        }

                        context.Add(item);
                        context.SaveChanges();
                    }

                }

                if (!context.MaterialColors.Any())
                {
                    var jsonData = File.ReadAllText(Path.Combine(pathToData, "Colors.json"));
                    List<MaterialColor> data = JsonSerializer.Deserialize<List<MaterialColor>>(jsonData)!;

                    foreach (var item in data)
                    {
                        context.Add(item);
                        context.SaveChanges();
                    }

                }

                if (!context.Materials.Any())
                {
                    var jsonData = File.ReadAllText(Path.Combine(pathToData, "Materials.json"));
                    var jsonConnectionBetwenMaterialsAndMaterials = File.ReadAllText(Path.Combine(pathToData, "MaterialColors.json"));

                    List<Material> data = JsonSerializer.Deserialize<List<Material>>(jsonData)!;
                    List<MaterialAndColors> dataColorsAndMaterials = JsonSerializer.Deserialize<List<MaterialAndColors>>(jsonConnectionBetwenMaterialsAndMaterials)!;

                    foreach (var item in data)
                    {
                        context.Add(item);
                        context.SaveChanges();
                    }

                    foreach (var item in dataColorsAndMaterials)
                    {
                        var color = context.MaterialColors.FirstOrDefault(c => c.Id == item.ColorId);
                        var material = context.Materials.FirstOrDefault(m => m.Id == item.MaterialId);

                        if(color != null && material != null)
                        {
                            material.Colors.Add(color);
                            context.Update(material);
                            context.SaveChanges();
                        }
                    }
                }

                AppRolesAndUsersSeeder appRolesAndUsersSeeder = new AppRolesAndUsersSeeder(
                    serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>()!,
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>()!,
                    serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!);
                
                await appRolesAndUsersSeeder.SeedDefaultRolesAndUsersIfEmptyAsync();
            }
        }
    }
}
