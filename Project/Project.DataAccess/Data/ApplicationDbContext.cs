using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set;}
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                 new Category { Id = 1, Name = "Dogs", DisplayOrder = 1 },
                 new Category { Id = 2, Name = "Cats", DisplayOrder = 2 },
                 new Category { Id = 3, Name = "Birds", DisplayOrder = 3 }
                );
            modelBuilder.Entity<Product>().HasData(
new Product
{
    Id = 1,
    Title = "Victor High Energy",
    Description = "Formulated for highly active dogs, this nutrient-dense kibble features high-quality protein from beef and chicken meals to support rigorous training demands.",
    Brand = "Victor",
    Weight = "230g",
    Type = "Dog Food",
    ListPrice = 33,
    Price = 30,
    Price50 = 27,
    Price100 = 20,
    CategoryId = 1,
    Image=""
},
new Product
{
    Id = 2,
    Title = "Royal Canin Indoor",
    Description = "Specially formulated for indoor cats to maintain a healthy weight and support digestive health with optimal fiber content.",
    Brand = "Royal Canin",
    Weight = "1.5kg",
    Type = "Cat Food",
    ListPrice = 45,
    Price = 40,
    Price50 = 37,
    Price100 = 30,
    CategoryId = 2,
    Image = ""
},
new Product
{
    Id = 3,
    Title = "Hill's Science Diet Adult",
    Description = "A balanced diet for adult dogs, featuring high-quality proteins to maintain lean muscle and omega fatty acids for healthy skin and coat.",
    Brand = "Hill's",
    Weight = "2.5kg",
    Type = "Dog Food",
    ListPrice = 50,
    Price = 45,
    Price50 = 42,
    Price100 = 38,
    CategoryId = 1,
    Image = ""
},
new Product
{
    Id = 4,
    Title = "Purina Pro Plan Cat Food",
    Description = "This cat food provides high protein from real chicken, designed to enhance your cat's immune system and support muscle strength.",
    Brand = "Purina Pro Plan",
    Weight = "1.2kg",
    Type = "Cat Food",
    ListPrice = 40,
    Price = 35,
    Price50 = 32,
    Price100 = 28,
    CategoryId = 2,
    Image = ""
},
new Product
{
    Id = 5,
    Title = "Wellness CORE Grain-Free",
    Description = "A protein-rich, grain-free formula for dogs, packed with premium meats, fruits, and vegetables for balanced nutrition.",
    Brand = "Wellness",
    Weight = "2.5kg",
    Type = "Dog Food",
    ListPrice = 60,
    Price = 55,
    Price50 = 52,
    Price100 = 48,
    CategoryId = 1,
    Image = ""
},
new Product
{
    Id = 6,
    Title = "Blue Buffalo Wilderness",
    Description = "A high-protein, grain-free formula made with real chicken and a blend of fruits and vegetables to provide balanced nutrition.",
    Brand = "Blue Buffalo",
    Weight = "1.5kg",
    Type = "Cat Food",
    ListPrice = 50,
    Price = 45,
    Price50 = 42,
    Price100 = 38,
    CategoryId = 2,
    Image = ""
},
new Product
{
    Id = 7,
    Title = "Orijen Original",
    Description = "Biologically appropriate dog food made with fresh, regional ingredients, rich in protein and nutrients for optimal health.",
    Brand = "Orijen",
    Weight = "2kg",
    Type = "Dog Food",
    ListPrice = 80,
    Price = 75,
    Price50 = 70,
    Price100 = 65,
    CategoryId = 1,
    Image = ""
},
new Product
{
    Id = 8,
    Title = "Taste of the Wild",
    Description = "A high-quality, grain-free cat food featuring real meat and antioxidants for optimal health and immune support.",
    Brand = "Taste of the Wild",
    Weight = "2kg",
    Type = "Cat Food",
    ListPrice = 45,
    Price = 40,
    Price50 = 38,
    Price100 = 35,
    CategoryId = 2,
    Image = ""
},
new Product
{
    Id = 9,
    Title = "Merrick Grain-Free Texas Beef ",
    Description = "Made with real deboned beef as the first ingredient, this grain-free formula provides balanced nutrition for all life stages.",
    Brand = "Merrick",
    Weight = "2.7kg",
    Type = "Dog Food",
    ListPrice = 70,
    Price = 65,
    Price50 = 62,
    Price100 = 58,
    CategoryId = 1,
    Image = ""
}

                );
        }

    }
}
