using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCellar.API.Context;
using MyCellar.API.Repository.Impl;
using MyCellar.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.Tests.Repository.Impl.Tests
{
    [TestClass()]
    public class ProductRepositoryTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var db = GetMemoryContext();
            db.Database.EnsureDeleted();
            db.Products.Add(new Product
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1",
            });
            db.Products.Add(new Product
            {
                Id = 2,
                Title = "PROD2",
                Description = "Description produit 2",
                Quantity = 3,
                ImgUrl = "ImgUrlPROD2",
            });
            db.SaveChanges();
        }

        [TestCleanup()] // S'execute avant chaque test
        public void Cleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public async Task TestGetAll()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            // Act
            var list = await repository.GetAll();

            // Assert
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod()]
        public async Task AddTest()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            // Act
            var productToSave = new Product()
            {
                Id = 3,
                Title = "PROD3",
                Description = "Description produit 3",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD3",
            };

            var productSaved = await repository.Add(productToSave);

            // Assert
            Assert.AreEqual(productToSave, productSaved);
        }

        [TestMethod()]
        public async Task CountTest()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            // Act
            var count = await repository.Count();

            // Assert
            Assert.AreEqual(2, count);
        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());
            var productToDelete = await repository.GetById(1);
            // Act
            await repository.Delete(productToDelete);

            var list = await repository.GetAll();

            // Assert
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod()]
        public async Task GetAllPaginateTest()
        {
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            var query = await repository.GetAllPaginate(1, 10, "prod1");

            Assert.AreEqual(1, query.Count);
        }

        [TestMethod()]
        public async Task GetByIdTest()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            // Act
            var product = await repository.GetById(1);

            // Assert
            Assert.AreEqual("PROD1", product?.Title);
        }

        [TestMethod()]
        public async Task UpdateTest()
        {
            // Arrange
            ProductRepository repository = new ProductRepository(GetMemoryContext());

            // Act
            var productToEdit = new Product()
            {
                Id = 2,
                Title = "PROD02",
                Description = "Description produit 02",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD02",
            };

            var productEdited = await repository.Update(productToEdit);

            // Assert
            Assert.AreEqual(productToEdit, productEdited);
        }

        private static ModelDbContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ModelDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
            return new ModelDbContext(options);
        }
    }
}