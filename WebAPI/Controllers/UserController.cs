using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.Models;
using Mit_Oersted.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        private readonly IConfiguration _config;

        public UserController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<User, UserDto> userMapper,
            ILogger<UserController> logger,
            IConfiguration config
            )
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        [HttpGet]
        [Route("api/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            List<User> list = _unitOfWork.Users.GetAllAsync().Result;

            if (list.Count <= 0) { return Ok("No users have been made yet"); }

            var result = new List<UserDto>();

            foreach (User item in list) { result.Add(_userMapper.Map(item)); }

            return Ok(result);
        }

        /// <summary>
        /// Gets a specifi user.
        /// </summary>
        /// <param name="id"></param>   
        /// <returns>A user by id</returns> 
        [HttpGet("api/users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw ExceptionFactory.UserWithIdNotFoundException(id); }

            User dbModel = GetUserByIdOrThrowException(id);

            return Ok(_userMapper.Map(dbModel));
        }

        /// <summary>
        /// Logs user in with phone number.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/login
        ///     {
        ///        "PhoneNumber": "+4588888888",
        ///        "Code": "123456"
        ///     }
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <returns>A newly created JWT token after login</returns>
        [HttpPost("api/users/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult LogUserInWithPhoneNumber([FromBody] LogUserInWithPhoneBodyDto body)
        {
            string sendVerificationCodeRespons = CallGoogleIdentitytoolkit($"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={ _config.GetSection("ProjectApiKey").Value }", new SendVerificationCodeBodyDto()
            {
                PhoneNumber = body.PhoneNumber
            }).Result;

            string signInWithPhoneNumberRespons = CallGoogleIdentitytoolkit($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={ _config.GetSection("ProjectApiKey").Value }", new SignInWithPhoneNumberBodyDto()
            {
                SessionInfo = GetSendVerificationCode(sendVerificationCodeRespons).SessionInfo,
                PhoneNumber = body.PhoneNumber,
                Code = body.Code,
                ReturnSecureToken = true
            }).Result;

            return Ok(GetSignInWithPhoneNumber(signInWithPhoneNumberRespons));
        }

        /// <summary>
        /// Refreshes a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/login
        ///     {
        ///        "grant_type": "refresh_token",
        ///        "refresh_token": "AOvuKvT7-x_6fI_92rtpCmqW6gWDFIcPTEvUB1_1MfxAwZUDuw49Mrw_pp6sDkDURfnHGzzobBH0a6cP1FSjdl66ybDe7XgaYPWLEyrAiN9GjPX2r1QmkWgQ-PdiLXMlBKf4hb98TO-tqDeOM0bkp1wUNpdDYDTiAwOhfxQTmBYXHNaO9EXrRryiaRBrOwIRbU86DD7eG5EN"
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns>A newly created JWT token after refresh</returns>
        [HttpPost("api/users/{id}/token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RefreshToken(string id, [FromBody] RefreshTokenBodyDto body)
        {
            string respons = CallGoogleIdentitytoolkit($"https://securetoken.googleapis.com/v1/token?key={ _config.GetSection("ProjectApiKey").Value }", body).Result;

            return Ok(GetRefreshToken(respons));
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks> 
        /// <param name="value"></param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user is null</response> 
        [HttpPost("api/users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult CreateNewUser([FromBody] string value)
        {
            return Created("", new object());
        }

        /// <summary>
        /// Updates a specifi user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/users/{id}
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param> 
        /// <param name="value"></param>  
        [HttpPut("api/users/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult UpdateUser(string id, [FromBody] string value)
        {
            return NoContent();
        }

        private User GetUserByIdOrThrowException(string id)
        {
            User dbModel = _unitOfWork.Users.GetByIdAsync(id).Result;

            if (dbModel == null) { throw ExceptionFactory.UserWithIdNotFoundException(id); }

            return dbModel;
        }

        private static SignInWithPhoneNumberResponseDto GetSignInWithPhoneNumber(string HttpResponseMessage)
        {
            return JsonSerializer.Deserialize<SignInWithPhoneNumberResponseDto>(HttpResponseMessage);
        }

        private static SendVerificationCodeResponseDto GetSendVerificationCode(string HttpResponseMessage)
        {
            return JsonSerializer.Deserialize<SendVerificationCodeResponseDto>(HttpResponseMessage);
        }

        private static RefreshTokenResponseDto GetRefreshToken(string HttpResponseMessage)
        {
            return JsonSerializer.Deserialize<RefreshTokenResponseDto>(HttpResponseMessage);
        }

        private static async Task<string> CallGoogleIdentitytoolkit(string url, object body)
        {
            try
            {
                string bodyString = JsonSerializer.Serialize(body);

                HttpClient client = new HttpClient();

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new StringContent(bodyString, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException hrex)
            {
                if (hrex.Message == "Response status code does not indicate success: 400 (Bad Request).")
                {
                    throw ExceptionFactory.ErrorWithGoogleAuthException(hrex.Message);
                }
                else
                {
                    throw new Exception(hrex.Message);
                }
            }
        }
    }
}
