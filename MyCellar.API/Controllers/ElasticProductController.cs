using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCellar.API.Repository;
using MyCellar.API.Models;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticProductController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticProductController> _logger;
        private readonly API.Repository.IRepository<Product> _productRepository;

        public ElasticProductController(
                        IElasticClient elasticClient,
                        ILogger<ElasticProductController> logger,
                        API.Repository.IRepository<Product> productRepository)
        {
            _logger = logger;
            _elasticClient = elasticClient;
            _productRepository = productRepository;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet(Name = "GetAllProducts")]
        public async Task<IActionResult> Get(string keyword, int page = 1, int pageSize = 5)
        {
            var result = await _elasticClient.SearchAsync<Product>(
                     s => s.Query(q => q.QueryString(d => d.Query('*' + keyword + '*')))
                     .From((page - 1) * pageSize)
                     .Size(pageSize));

            if (!result.IsValid)
            {
                // We could handle errors here by checking response.OriginalException 
                //or response.ServerError properties
                _logger.LogError("Failed to search documents");
                return Ok(new Product[] { });
            }

            _logger.LogInformation("ProductsController Get - ", DateTime.UtcNow);
            return Ok(result.Documents);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "reindex")]
        public async Task<IActionResult> Post()
        {
            await _elasticClient.DeleteByQueryAsync<Product>(q => q.MatchAll());

            var products = await _productRepository.GetAll();

            foreach (var product in products)
            {
                await _elasticClient.IndexDocumentAsync(product);
            }

            _logger.LogInformation("ProductsController Loading data - ", DateTime.UtcNow);
            return Ok($"{products.Count()} product(s) reindexed");
        }
    }
}
