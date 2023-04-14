using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCellar.API.Models;
using MyCellar.API.Models.Enums;
using BC = BCrypt.Net.BCrypt;


namespace MyCellar.API.Context
{
    public class ModelDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }

        public ModelDbContext(DbContextOptions<ModelDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=HRMan;integrated security=True;");
        //    var connectionString = "server=localhost;user=root;password=root;database=mycellardb";
        //    var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
        //    optionsBuilder.UseMySql(connectionString, serverVersion)
        //                .LogTo(Console.WriteLine, LogLevel.Information)
        //                .EnableSensitiveDataLogging()
        //                .EnableDetailedErrors();

        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Utilise Fluent API pour configurer notre Db, cad, tables, index et relation

            // Mappings vers le tables & Declare PK
            modelBuilder.Entity<Category>().ToTable("categories").HasKey(u => u.Id).HasName("PK_Categories");
            modelBuilder.Entity<Product>().ToTable("products").HasKey(u => u.Id).HasName("PK_Products");
            modelBuilder.Entity<Recipe>().ToTable("recipes").HasKey(u => u.Id).HasName("PK_Recipes");
            modelBuilder.Entity<RecipeProduct>().ToTable("recipes_products").HasKey(c => new { c.RecipeId, c.ProductId }).HasName("PK_Recipes_Products");
            modelBuilder.Entity<User>().ToTable("users").HasKey(u => u.Id).HasName("PK_Users");
            modelBuilder.Entity<UserProduct>().ToTable("users_products").HasKey(c => new { c.UserId, c.ProductId }).HasName("PK_Users_Products");

            // Configuration des tables
            // Category
            modelBuilder.Entity<Category>().Property(u => u.Id).HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            modelBuilder.Entity<Category>().Property(u => u.Title).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Category>().Property(u => u.Description).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Category>().Property(u => u.CreatedDate).HasColumnType("datetime").IsRequired();
            //modelBuilder.Entity<Category>().Property(u => u.UpdatedDate).HasColumnType("datetime").IsRequired(false);

            // Product
            modelBuilder.Entity<Product>().Property(u => u.Id).HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.Title).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.Description).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.Quantity).HasColumnType("int(15)").IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.ImgUrl).HasColumnType("nvarchar(100)").IsRequired(false);
            modelBuilder.Entity<Product>().Property(u => u.CreatedDate).HasColumnType("datetime").IsRequired();
            //modelBuilder.Entity<Product>().Property(u => u.UpdatedDate).HasColumnType("datetime").IsRequired(false);

            // Recipe
            modelBuilder.Entity<Recipe>().Property(u => u.Id).HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.Title).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.Description).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.Duration).HasColumnType("int(15)").IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.Difficulty).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.Caloric).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<Recipe>().Property(u => u.CreatedDate).HasColumnType("datetime").IsRequired();
            //modelBuilder.Entity<Recipe>().Property(u => u.UpdatedDate).HasColumnType("datetime").IsRequired();

            // RecipeProduct
            modelBuilder.Entity<RecipeProduct>().Property(u => u.RecipeId).HasColumnType("int").IsRequired();
            modelBuilder.Entity<RecipeProduct>().Property(u => u.ProductId).HasColumnType("int").IsRequired();

            // User
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            modelBuilder.Entity<User>().Property(u => u.UserName).HasColumnType("nvarchar(100)").IsRequired(false);
            modelBuilder.Entity<User>().Property(u => u.Email).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Password).HasColumnType("nvarchar(100)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Sexe).HasColumnType("nvarchar(100)").IsRequired(false);
            modelBuilder.Entity<User>().Property(u => u.Role).HasColumnType("nvarchar(100)").IsRequired(false);


            // UserProduct
            modelBuilder.Entity<UserProduct>().Property(u => u.UserId).HasColumnType("int").IsRequired();
            modelBuilder.Entity<UserProduct>().Property(u => u.ProductId).HasColumnType("int").IsRequired();

            // Relation OneToMany
            // Une categorie peut avoir un ou plusieurs produits
            modelBuilder.Entity<Product>().HasOne<Category>().WithMany(g => g.Products).HasPrincipalKey(u => u.Id)
               .HasForeignKey(u => u.Id).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Categories_ProductsGroup");

            // Relation ManyToMany
            // Une recette peut avoir sur un ou plusieurs ingredients
            // Un ingredient peut etre dans une ou plusieurs recettes
            modelBuilder.Entity<RecipeProduct>().HasOne(sc => sc.Recipe).WithMany(s => s.RecipeProducts).HasForeignKey(sc => sc.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RecipeProduct>().HasOne(sc => sc.Product).WithMany(s => s.RecipeProducts).HasForeignKey(sc => sc.ProductId).OnDelete(DeleteBehavior.Cascade);

            // Un utilisateur peut avoir dans sa box un ou plusieurs produits
            // Un ingredient peut etre checke dans une ou plusieurs utilisateurs
            modelBuilder.Entity<UserProduct>().HasOne(sc => sc.User).WithMany(s => s.UserProducts).HasForeignKey(sc => sc.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserProduct>().HasOne(sc => sc.Product).WithMany(s => s.UserProducts).HasForeignKey(sc => sc.ProductId).OnDelete(DeleteBehavior.Cascade);

            // Seeding Data
            modelBuilder.Entity<Category>().HasData(
               new Category
               {
                   Id = 1,
                   Title = "CAT1",
                   Description = "Description cat 1",
                   CreatedDate = new DateTime().Date
               },
               new Category
               {
                   Id = 2,
                   Title = "CAT2",
                   Description = "Description cat 2",
                   CreatedDate = new DateTime().Date
               },
               new Category
               {
                   Id = 3,
                   Title = "CAT3",
                   Description = "Description cat 3",
                   CreatedDate = new DateTime().Date
               },
               new Category
               {
                   Id = 4,
                   Title = "CAT4",
                   Description = "Description cat 4",
                   CreatedDate = new DateTime().Date
               }
           );

            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   Id = 1,
                   Title = "PROD1",
                   Description = "Description produit 1",
                   Quantity = 3,
                   ImgUrl = "ImgUrlPROD1",
                   CreatedDate = new DateTime().Date,
                   CategoryId = 1
               },
               new Product
               {
                   Id = 2,
                   Title = "PROD2",
                   Description = "Description produit 2",
                   Quantity = 10,
                   ImgUrl = "ImgUrlPROD2",
                   CreatedDate = new DateTime().Date,
                   CategoryId = 1
               },
               new Product
               {
                   Id = 3,
                   Title = "PROD3",
                   Description = "Description produit 3",
                   Quantity = 2,
                   ImgUrl = "ImgUrlPROD3",
                   CreatedDate = new DateTime().Date,
                   CategoryId = 2
               },
               new Product
               {
                   Id = 4,
                   Title = "PROD4",
                   Description = "Description produit 4",
                   Quantity = 1,
                   ImgUrl = "ImgUrlPROD4",
                   CreatedDate = new DateTime().Date,
                   CategoryId = 2
               }
           );

            modelBuilder.Entity<Recipe>().HasData(
              new Recipe
              {
                  Id = 1,
                  Title = "RECETTE1",
                  Description = "Description recette 1",
                  Duration = 3,
                  ImgUrl = "ImgUrlRECETTE1",
                  Difficulty = Difficulty.Medium,
                  Caloric = Caloric.High,
                  CreatedDate = new DateTime().Date
              },
              new Recipe
              {
                  Id = 2,
                  Title = "RECETTE2",
                  Description = "Description recette 2",
                  Duration = 3,
                  ImgUrl = "ImgUrlRECETTE2",
                  Difficulty = Difficulty.Medium,
                  Caloric = Caloric.High,
                  CreatedDate = new DateTime().Date
              },
              new Recipe
              {
                  Id = 3,
                  Title = "RECETTE3",
                  Description = "Description recette 3",
                  Duration = 3,
                  ImgUrl = "ImgUrlRECETTE3",
                  Difficulty = Difficulty.Medium,
                  Caloric = Caloric.High,
                  CreatedDate = new DateTime().Date
              },
              new Recipe
              {
                  Id = 4,
                  Title = "RECETTE4",
                  Description = "Description recette 4",
                  Duration = 3,
                  ImgUrl = "ImgUrlRECETTE4",
                  Difficulty = Difficulty.Medium,
                  Caloric = Caloric.High,
                  CreatedDate = new DateTime().Date
              }
           );

            modelBuilder.Entity<RecipeProduct>().HasData(
              new RecipeProduct
                {
                    RecipeId = 1,
                    ProductId = 2
                },
              new RecipeProduct
                {
                    RecipeId = 1,
                    ProductId = 3
                },
              new RecipeProduct
                 {
                     RecipeId = 1,
                     ProductId = 1
                 },
              new RecipeProduct
                 {
                     RecipeId = 2,
                     ProductId = 4
                 }
           );

            modelBuilder.Entity<User>().HasData(
                   new User
                   {
                       Id = 1,
                       UserName = "prenom1",                   
                       Email = "email1@email.fr",
                       Password = BC.HashPassword("password"),
                       Sexe = "sexe1",                  
                       Role = "User",
                       CreatedDate = new DateTime().Date
                   },
                   new User
                   {
                       Id = 2,
                       UserName = "prenom2",
                       Email = "email2@email.fr",
                       Password = BC.HashPassword("password"),
                       Sexe = "sexe1",
                       Role = "Admin",
                       CreatedDate = new DateTime().Date
                   }
               ); 
        }
    }
}
