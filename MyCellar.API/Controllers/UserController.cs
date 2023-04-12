using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCellar.API.Repository;
using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyCellar.API.Utils;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a current user
        /// </summary>
        /// <response code="200">Cuurent user retrieved</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
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
                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.GetById(int.Parse(userId))
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            else
            {
                return NotFound(new CustomResponse<Error>
                {
                    Message = Global.ResponseMessages.Forbidden,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Unauthorized !") }
                });
            }
        }

        /// <summary>
        /// Retrieves a current user
        /// </summary>
        /// <response code="200">Current user retrieved</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User")]
        [HttpPut("current")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] User user)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
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
                    var userToUpdate = await _userRepository.GetById(int.Parse(userId));
                    userToUpdate.Email = user.Email;
                    userToUpdate.UserName = user.UserName;

                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.Update(userToUpdate)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            else
            {
                return NotFound(new CustomResponse<Error>
                {
                    Message = Global.ResponseMessages.Forbidden,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Unauthorized !") }
                });
            }
        }

        /// <summary>
        /// Retrieves all products from current user
        /// </summary>
        /// <response code="200">Products retrieved</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User")]
        [HttpGet("current/products")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllProductsFromCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
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
                    return Ok(new CustomResponse<List<Product>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.GetAllProductsFromCurrentUser(int.Parse(userId))
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            else
            {
                return NotFound(new CustomResponse<Error>
                {
                    Message = Global.ResponseMessages.Forbidden,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Unauthorized !") }
                });
            }
        }

        /// <summary>
        /// Assign a specific product to the current user
        /// </summary>
        /// <response code="200">Product assigned</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User")]
        [HttpPost("current/products")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AssignProductToCurrentUser([FromBody] int productId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
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
                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.AssignOneProductToCurrentUser(int.Parse(userId), productId)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            else
            {
                return NotFound(new CustomResponse<Error>
                {
                    Message = Global.ResponseMessages.Forbidden,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Unauthorized !") }
                });
            }
        }

        /// <summary>
        /// Delete a specific product by current user
        /// </summary>
        /// <response code="200">Product deleted</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [Authorize(Roles = "User")]
        [HttpDelete("current/products/{productId}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteProductToCurrentUser(int productId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null) { 
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
                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.DeleteOneProductFromCurrentUser(int.Parse(userId), productId)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }

            } else
            {
                return NotFound(new CustomResponse<Error>
                {
                    Message = Global.ResponseMessages.Forbidden,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Unauthorized !") }
                });
            }
        }
    }
}
