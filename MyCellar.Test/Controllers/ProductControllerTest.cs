
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyCellar.API.Controllers;
using MyCellar.API.Utils;
using MyCellar.API.Repository;
using MyCellar.API.Wrappers;
using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.Tests.Controllers.Tests
{
    [TestClass()]
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;

        private readonly ProductController _controller;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _mockMapper = new Mock<IMapper>();

            _controller = new ProductController(_mockRepo.Object, _mockMapper.Object);
        }

        [TestMethod()]
        public async Task GetAllTest()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockRepo.Setup(x => x.GetAll()).Returns(Task.FromResult(products));

            // Act
            var actionResult = await _controller.GetAll();
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<List<Product>>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual.Result.Count(), products.Count);
        }

        [TestMethod()]
        public async Task GetTest()
        {
            // Arrange
            var product = new Product 
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1"
            };
            _mockRepo.Setup(x => x.GetById(product.Id)).Returns(Task.FromResult(product));

            // Act
            var actionResult = await _controller.Get(product.Id);
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<Product>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual?.Result.Title, product.Title);
        }

        [TestMethod()]
        public async Task PostTest()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1"
            };
            _mockRepo.Setup(x => x.Add(product)).Returns(Task.FromResult(product));

            // Act
            var actionResult = await _controller.Post(product);
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<Product>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual?.Result.Title, product.Title);
        }

        [TestMethod()]
        public void ValidateModelErrorTest()
        {
            // Arrange
            var product = new Product
            {
                Title = null,
                Description = null,
                Quantity = 0,
                ImgUrl = null   
            };

            // Assert
            Assert.IsTrue(ValidateModel(product).Any(
                v => v.MemberNames.Contains("Title") &&
                     v.ErrorMessage.Contains("required")));

        }

        [TestMethod()]
        public async Task PutTest()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1"
            };
            _mockRepo.Setup(x => x.GetById(product.Id)).Returns(Task.FromResult(product));

            _mockRepo.Setup(x => x.Update(product)).Returns(Task.FromResult(product));

            // Act
            var actionResult = await _controller.Put(1, product);
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<Product>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual?.Result.Title, product.Title);
        }

        [TestMethod()]
        public async Task PatchTest()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1"
            };
            _mockRepo.Setup(x => x.GetById(product.Id)).Returns(Task.FromResult(product));

            _mockRepo.Setup(x => x.Update(product)).Returns(Task.FromResult(product));

            JsonPatchDocument<Product> jsonPatchProduct = new JsonPatchDocument<Product>().Replace(x => x.Title, "PROD01");
            // Act
            var actionResult = await _controller.Patch(1, jsonPatchProduct);
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<Product>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual?.Result.Title, "PROD01");
        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "PROD1",
                Description = "Description produit 1",
                Quantity = 1,
                ImgUrl = "ImgUrlPROD1"
            };
            _mockRepo.Setup(x => x.GetById(product.Id)).Returns(Task.FromResult(product));

            _mockRepo.Setup(x => x.Delete(product)).Returns(Task.FromResult(product));

            // Act
            var actionResult = await _controller.Delete(1);
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<string>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual?.Result, "Deleted Successfully !");
        }

        [TestMethod()]
        public async Task CountTest()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockRepo.Setup(x => x.Count()).Returns(Task.FromResult(products.Count));

            // Act
            var actionResult = await _controller.Count();
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<int>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual.Result, products.Count);
        }

        [TestMethod()]
        public async Task GetPaginateTest()
        {
            // Arrange
            PageResult<Product> mockPageResult = new PageResult<Product>();
            _mockRepo.Setup(x => x.GetAllPaginate(0, 10, "PROD1")).Returns(Task.FromResult(mockPageResult));

            // Act
            var actionResult = await _controller.GetPaginate(0, 10, "PROD1");
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as CustomResponse<PageResult<Product>>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual.Result, mockPageResult);
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}