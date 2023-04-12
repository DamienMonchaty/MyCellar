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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IRepository<Product> productRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of categories
        /// </summary>
        /// <response code="200">Categories retrieved</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new CustomResponse<List<Category>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.GetAll()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Retrieves a specific category by unique id
        /// </summary>
        /// <response code="200">Category retrieved</response>
        /// <response code="404">Category not found</response>
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
                var c = await _categoryRepository.GetById(id);
                if (c != null)
                {
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = c
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Save a specific category
        /// </summary>
        /// <response code="201">Category created</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Post([FromBody] Category category)
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

                Category cToSave = _mapper.Map<Category>(category);

                return Ok(new CustomResponse<Category>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status201Created,
                    Result = await _categoryRepository.Add(cToSave)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Edit a specific category by unique id
        /// </summary>
        /// <response code="200">Category edited</response>
        /// <response code="404">Category not found</response>
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
        public async Task<IActionResult> Put(int id, [FromBody] Category category)
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

                var cToUpdate = await _categoryRepository.GetById(id);
                if (cToUpdate != null)
                {
                    //cToUpdate = _mapper.Map<Category, Category>(category, cToUpdate);
                    cToUpdate.Title = category.Title;
                    cToUpdate.Description = category.Description;
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _categoryRepository.Update(cToUpdate)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Patched a specific category by unique id
        /// </summary>
        /// <response code="200">Category patched</response>
        /// <response code="404">Category not found</response>
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
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Category> patchCategory)
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

                var cToEdit = await _categoryRepository.GetById(id);
                if (cToEdit != null)
                {
                    patchCategory.ApplyTo(cToEdit);
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _categoryRepository.Update(cToEdit)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Deleted a specific category by unique id
        /// </summary>
        /// <response code="200">Category deleted</response>
        /// <response code="404">Category not found</response>
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
                var cToDelete = await _categoryRepository.GetById(id);
                if (cToDelete != null)
                {
                    await _categoryRepository.Delete(cToDelete);
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
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
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
        /// <response code="200">Categories counted</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User, Admin")]
        [HttpGet("count")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Count()
        {
            try
            {
                return Ok(new CustomResponse<int>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.Count()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Retrieves a list of categories paginate, filter
        /// </summary>
        /// <response code="200">Categories retrieved by paginate & filter</response>
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
                return Ok(new CustomResponse<PageResult<Category>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.GetAllPaginate(page, pagesize, search)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Retrieves a list of products by categoryId
        /// </summary>
        /// <response code="200">Products retrieved</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{categoryId}/products")]
        [ProducesResponseType(typeof(List<Category>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<IEnumerable<Category>>> GetProductsByCategoryId(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetById(categoryId);
                if (category != null)
                {
                    return Ok(new CustomResponse<List<Category>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _categoryRepository.GetProductsByCategoryId(category.Id)
                });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + categoryId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Save a specific product by categoryId
        /// </summary>
        /// <response code="201">Product created</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("{categoryId}/products")]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Category>> SaveProductByCategoryId(int categoryId, [FromBody] Product product)
        {
            try
            {
                var category = await _categoryRepository.GetById(categoryId);
                if (category != null)
                {
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status201Created,
                        Result = await _categoryRepository.SaveOneProductByCategoryId(category.Id, product)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + categoryId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Get a specific product by productId && categoryId
        /// </summary>
        /// <response code="200">Product retrieved</response>
        /// <response code="404">Product not found</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{categoryId}/products/{productId}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Category>> GetProductByCategoryId(int categoryId, int productId)
        {
            try
            {
                var category = await _categoryRepository.GetById(categoryId);
                if (category != null)
                {
                    var product = await _productRepository.GetById(productId);
                    if (product != null)
                    {
                        return Ok(new CustomResponse<Product>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = StatusCodes.Status200OK,
                            Result = await _categoryRepository.GetOneProductByCategoryId(category.Id, product.Id)
                        });
                    }
                    else
                    {
                        return NotFound(new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.NotFound,
                            StatusCode = StatusCodes.Status404NotFound,
                            Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + productId + " is not found") }
                        });
                    }
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + categoryId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Edit a specific product by productId && categoryId
        /// </summary>
        /// <response code="200">Product edited</response>
        /// <response code="404">Product not found</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryId}/products/{productId}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Category>> UpdateProductByCategoryId(int categoryId, int productId, [FromBody] Product product)
        {
            try
            {
                var category = await _categoryRepository.GetById(categoryId);
                if (category != null)
                {
                    var productToEdit = await _productRepository.GetById(productId);
                    if (productToEdit != null)
                    {
                        return Ok(new CustomResponse<Category>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = StatusCodes.Status200OK,
                            Result = await _categoryRepository.EditOneProductByCategoryId(category.Id, productId, product)
                        });
                    }
                    else
                    {
                        return NotFound(new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.NotFound,
                            StatusCode = StatusCodes.Status404NotFound,
                            Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + productId + " is not found") }
                        });
                    }
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + categoryId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Delete a specific product by productId && categoryId
        /// </summary>
        /// <response code="200">Product deleted</response>
        /// <response code="404">Product not found</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId}/products/{productId}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Category>> DeleteProductByCategoryId(int categoryId, int productId)
        {
            try
            {
                var category = await _categoryRepository.GetById(categoryId);
                if (category != null)
                {
                    var product = await _productRepository.GetById(productId);
                    if (product != null)
                    {
                        return Ok(new CustomResponse<Category>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = StatusCodes.Status200OK,
                            Result = await _categoryRepository.DeleteOneProductByCategoryId(category.Id, product.Id)
                        });
                    }
                    else
                    {
                        return NotFound(new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.NotFound,
                            StatusCode = StatusCodes.Status404NotFound,
                            Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + productId + " is not found") }
                        });
                    }
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + categoryId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }
    }
}