using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyCellar.API.Utils;
using Microsoft.AspNetCore.Authorization;
using MyCellar.API.Repository;
using MyCellar.API.Wrappers;
using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of products
        /// </summary>
        /// <response code="200">Products retrieved</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Product>), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new CustomResponse<List<Product>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _productRepository.GetAll()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <response code="200">Product retrieved</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var a = await _productRepository.GetById(id);
                if (a != null)
                {
                    return Ok(new CustomResponse<Product>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = a
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Save a specific product
        /// </summary>
        /// <response code="201">Products created</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomResponse<object>
                    {
                        Message = Global.ResponseMessages.BadRequest,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Result = ModelState
                    });
                }
                else
                {
                    // Product pToSave = _mapper.Map<Product>(product);
                    return Ok(new CustomResponse<Product>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _productRepository.Add(product)
                    });
                }
              
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Edit a specific product by unique id
        /// </summary>
        /// <response code="200">Products edited</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomResponse<object>
                    {
                        Message = Global.ResponseMessages.BadRequest,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Result = ModelState
                    });
                }

                var pToUpdate = await _productRepository.GetById(id);
                if (pToUpdate != null)
                {
                    //pToUpdate = _mapper.Map(product, pToUpdate);
                    pToUpdate.Title = product.Title;
                    pToUpdate.Description = product.Description;
                    pToUpdate.ImgUrl = product.ImgUrl;
                    pToUpdate.Quantity = product.Quantity;
                    return Ok(new CustomResponse<Product>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _productRepository.Update(pToUpdate)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Patched a specific product by unique id
        /// </summary>
        /// <response code="200">Products patched</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Product> patchProduct)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomResponse<object>
                    {
                        Message = Global.ResponseMessages.BadRequest,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Result = ModelState
                    });
                }

                var pToEdit = await _productRepository.GetById(id);
                if (pToEdit != null)
                {
                    patchProduct.ApplyTo(pToEdit);
                    return Ok(new CustomResponse<Product>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _productRepository.Update(pToEdit)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Deleted a specific product by unique id
        /// </summary>
        /// <response code="200">Products deleted</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pToDelete = await _productRepository.GetById(id);
                if (pToDelete != null)
                {
                    await _productRepository.Delete(pToDelete);
                    return Ok(new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = "Deleted Successfully !"
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Count all occurences
        /// </summary>
        /// <response code="200">Products counted</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet("count")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Count()
        {
            try
            {
                return Ok(new CustomResponse<int>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _productRepository.Count()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Retrieves a list of products paginate, filter
        /// </summary>
        /// <response code="200">Products retrieved by paginate & filter</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet("paginate")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPaginate([FromQuery] int? page, [FromQuery] int pagesize = 10, [FromQuery] string search = "")
        {
            try
            {
                return Ok(new CustomResponse<PageResult<Product>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _productRepository.GetAllPaginate(page, pagesize, search)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }
    }
}