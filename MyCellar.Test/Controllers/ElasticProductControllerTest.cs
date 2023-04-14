using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyCellar.API.Controllers;
using MyCellar.API.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCellar.Tests.Controllers
{
    [TestClass()]
    public class ElasticProductControllerTest
    {
        private readonly Mock<IElasticClient> _mockElasticClient;
        private readonly Mock<ILogger<ElasticProductController>> _mockLogger;
        private readonly Mock<API.Repository.IRepository<Product>> _mockRepo;

        private readonly ElasticProductController _controller;

        public ElasticProductControllerTest()
        {
            _mockElasticClient = new Mock<IElasticClient>();
            _mockLogger = new Mock<ILogger<ElasticProductController>>();
            _mockRepo = new Mock<API.Repository.IRepository<Product>>();

            _controller = new ElasticProductController(_mockElasticClient.Object, _mockLogger.Object, _mockRepo.Object);
        }

        //[TestMethod()]
        //public void GetTest()
        //{
        //    // Arrange
        //    var products = new List<Product> { new Product(), new Product() };
        //    var mockSearchResponse = new Mock<ISearchResponse<Product>>();
        //    mockSearchResponse.Setup(x => x.Documents).Returns(products);

        //    _mockElasticClient.Setup(x => x
        //        .Search(It.IsAny<Func<SearchDescriptor<Product>, ISearchRequest>>()))
        //        .Returns(mockSearchResponse.Object);

        //    var result = _mockElasticClient.Object.Search<Product>(s => s);

        //    Assert.AreEqual(2, result.Documents.Count);

        //}


        [TestMethod()]
        public async Task GetTest()
        {
            // Arrange
            var products = new List<Product> {
                new Product()
                {
                    Id = 1,
                    Title = "PROD1",
                    Description = "Description produit 1",
                    Quantity = 1,
                    ImgUrl = "ImgUrlPROD1",
                },
                new Product()
                {
                    Id = 2,
                    Title = "PROD2",
                    Description = "Description produit 2",
                    Quantity = 1,
                    ImgUrl = "ImgUrlPROD2",
                }
            };
            var mockSearchResponse = new Mock<ISearchResponse<Product>>();
            mockSearchResponse.Setup(x => x.Documents).Returns(products);

            _mockElasticClient.Setup(x => x
                .SearchAsync(It.IsAny<Func<SearchDescriptor<Product>, ISearchRequest>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockSearchResponse.Object));

            // Act
            var actionResult = await _controller.Get("PROD1");
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as ISearchResponse<Product>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Assert.AreEqual(actual.Total, 1);

        }

        [TestMethod()]
        public async Task PostTest()
        {
            // Arrange
            _mockElasticClient.Setup(x => x
                .DeleteByQueryAsync((Func<DeleteByQueryDescriptor<Product>, IDeleteByQueryRequest>)It.IsAny<Func<SearchDescriptor<Product>, ISearchRequest>>(), It.IsAny<CancellationToken>()));

            var products = new List<Product> {
                new Product()
                {
                    Id = 1,
                    Title = "PROD1",
                    Description = "Description produit 1",
                    Quantity = 1,
                    ImgUrl = "ImgUrlPROD1",
                },
                new Product()
                {
                    Id = 2,
                    Title = "PROD2",
                    Description = "Description produit 2",
                    Quantity = 1,
                    ImgUrl = "ImgUrlPROD2",
                }
            };

            _mockRepo.Setup(x => x.GetAll()).Returns(Task.FromResult(products));

            var p = await _mockRepo.Object.GetAll();

            var mockSearchResponse = new Mock<ISearchResponse<Product>>();
            mockSearchResponse.Setup(x => x.Documents).Returns(products);

            var mockResponse = new Mock<IndexResponse>();

            _mockElasticClient.Setup(x => x
                .IndexDocumentAsync(It.IsAny<Func<IndexDescriptor<Product>, IIndexRequest<Product>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockResponse.Object));

            // Act
            var actionResult = await _controller.Post();
            var result = actionResult as OkObjectResult;
            var actual = result?.Value as string;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(actual, "2 product(s) reindexed");

        }

    }
}
