using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mit_Oersted.WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mit_Oersted.WebAPI.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<User, UserDto> _userMapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<User, UserDto> userMapper,
            ILogger<UserController> logger
            )
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userMapper = userMapper ?? throw new System.ArgumentNullException(nameof(userMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        [HttpGet]
        [Route("api/users")]
        //[Authorize]
        public IActionResult GetAll()
        {
            List<User> list = _unitOfWork.Users.GetAllAsync().Result;

            if (list.Count > 0) { Ok("No user request have been made yet"); }

            var result = new List<UserDto>();

            foreach (User item in list) { result.Add(_userMapper.Map(item)); }

            return Ok(result);
        }

        /// <summary>
        /// Gets a specifi user.
        /// </summary>
        /// <param name="id"></param>    
        [HttpGet("api/users/{id}")]
        //[Authorize]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        /// <summary>
        /// Gets a specifi user.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user is null</response> 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("api/users")]
        //[Authorize]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// Updates a specifi user.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="value"></param>  
        [HttpPut("api/users/{id}")]
        //[Authorize]
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}
