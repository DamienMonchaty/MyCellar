using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCellar.API.Models;
using MyCellar.API.JwtHelpers;
using MyCellar.API.Messages.Requests;
using MyCellar.API.Utils;
using MyCellar.API.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public IConfiguration _configuration { get; }
        private readonly IUserRepository _userRepository;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Login by username && password
        /// </summary>
        /// <response code="200">User logged</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Login(LoginUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userRepository.GetByUserName(model.UserName);

                if (user != null || BC.Verify(model.Password, user.Password))
                {
                    var token = user.GenerateToken(_configuration);
                    return Ok(new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = token
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.Forbidden,
                        StatusCode = StatusCodes.Status403Forbidden,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("username or password") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        /// <summary>
        /// Register an user
        /// </summary>
        /// <response code="200">User created</response>
        /// <response code="500">Oops! Error</response>
        /// <response code="401">Unauthorized !</response>
        /// <response code="403">Access Restricted !</response>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (user.Role is null) {
                    user.Role = "User";
                }
                return Ok(new CustomResponse<User>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status201Created,
                    Result = await _userRepository.Add(user)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

    }
}
