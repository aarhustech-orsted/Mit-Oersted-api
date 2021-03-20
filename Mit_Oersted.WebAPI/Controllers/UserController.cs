using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mit_Oersted.Domain.Commands.Users;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Models;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Models.Tokens;
using Mit_Oersted.WebApi.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mit_Oersted.WebApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<UserModel, UserDto> _userMapper;
        private readonly IMapper<RefreshTokenResponseDto, TokenResponseBodyDto> _tokenRefreshMapper;
        private readonly IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto> _signInWithPhoneNumberMapper;
        private readonly IConfiguration _config;

        public UserController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<UserModel, UserDto> userMapper,
            IMapper<RefreshTokenResponseDto, TokenResponseBodyDto> tokenRefreshMapper,
            IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto> signInWithPhoneNumberMapper,
            IConfiguration config
            )
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
            _tokenRefreshMapper = tokenRefreshMapper ?? throw new ArgumentNullException(nameof(tokenRefreshMapper));
            _signInWithPhoneNumberMapper = signInWithPhoneNumberMapper ?? throw new ArgumentNullException(nameof(signInWithPhoneNumberMapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <remarks> 
        /// Sample response:
        ///
        ///     GET /api/users
        ///     [
        ///        {
        ///            "id": "HYIlwOHnLR7cuUFY82Xj",
        ///            "name": "Gudrun Mortensen",
        ///            "email": "kareah@mixalo.com",
        ///            "address": "SGFzc2VsYWdlciBBbGzDqSAxMHx8ODI2MCBWaWJ5IEo=",
        ///            "phone": "+4588888888"
        ///        }
        ///     ]
        ///
        /// </remarks> 
        /// <returns>A list of users</returns> 
        [HttpGet("api/users")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            List<UserModel> list = _unitOfWork.Users.GetAllAsync().Result;

            if (list.Count <= 0) { return Ok("No users have been made yet"); }

            return base.Ok((from UserModel item in list
                            select _userMapper.Map(item)).ToList());
        }

        /// <summary>
        /// Gets a specifi user.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/users/HYIlwOHnLR7cuUFY82Xj
        ///     {
        ///        "id": "HYIlwOHnLR7cuUFY82Xj",
        ///        "name": "Gudrun Mortensen",
        ///        "email": "kareah@mixalo.com",
        ///        "address": "SGFzc2VsYWdlciBBbGzDqSAxMHx8ODI2MCBWaWJ5IEo=",
        ///        "phone": "+4588888888"
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param>   
        /// <returns>A user by id</returns> 
        [HttpGet("api/users/{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw ExceptionFactory.UserWithIdNotFoundException(id); }

            return base.Ok(_userMapper.Map(GetUserByIdOrThrowException(id)));
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
        /// Sample response:
        ///
        ///     POST /api/users/login
        ///     {
        ///        "refreshToken": string,
        ///        "idToken": string
        ///     }
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <returns>A newly created JWT token after login</returns>
        /// <response code="200"></response>
        /// <response code="400">If the body is null</response> 
        [HttpPost("api/users/login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TokenResponseBodyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult LogUserInWithPhoneNumber([FromBody] LogUserInWithPhoneBodyDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            var webapidata = JsonSerializer.Deserialize<Webapidata>(System.IO.File.ReadAllText(_config.GetSection("webapi").Value));

            string sendVerificationCodeRespons = CallGoogleIdentitytoolkit($"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={ webapidata.ProjectApiKey }", new SendVerificationCodeBodyDto()
            {
                PhoneNumber = body.PhoneNumber
            }).Result;

            string signInWithPhoneNumberRespons = CallGoogleIdentitytoolkit($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={ webapidata.ProjectApiKey }", new SignInWithPhoneNumberBodyDto()
            {
                SessionInfo = GetSendVerificationCode(sendVerificationCodeRespons).SessionInfo,
                PhoneNumber = body.PhoneNumber,
                Code = body.Code,
                ReturnSecureToken = true
            }).Result;

            return Ok(_signInWithPhoneNumberMapper.Map(GetSignInWithPhoneNumber(signInWithPhoneNumberRespons)));
        }

        /// <summary>
        /// Refreshes a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/HYIlwOHnLR7cuUFY82Xj/token
        ///     {
        ///        "grant_type": "refresh_token",
        ///        "refresh_token": string
        ///     }
        ///     
        /// Sample response:
        ///
        ///     POST /api/users/HYIlwOHnLR7cuUFY82Xj/token
        ///     {
        ///        "refreshToken": string,
        ///        "idToken": string
        ///     }
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <returns>A newly created JWT token after refresh</returns>
        /// <response code="200"></response>
        /// <response code="400">If the body is null</response> 
        [HttpPost("api/users/{id}/token")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TokenResponseBodyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult RefreshToken([FromBody] RefreshTokenBodyDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            var webapidata = JsonSerializer.Deserialize<Webapidata>(System.IO.File.ReadAllText(_config.GetSection("webapi").Value));

            return base.Ok(_tokenRefreshMapper.Map(GetRefreshToken(CallGoogleIdentitytoolkit($"https://securetoken.googleapis.com/v1/token?key={ webapidata.ProjectApiKey }", body).Result)));
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users
        ///     {
        ///        "name": "Gudrun Mortensen",
        ///        "email": "kareah@mixalo.com",
        ///        "address": "SGFzc2VsYWdlciBBbGzDqSAxMHx8ODI2MCBWaWJ5IEo=",
        ///        "phone": "+4588888888"
        ///     }
        ///     
        /// Sample response:
        ///
        ///     POST /api/users
        ///     {
        ///        "name": "Gudrun Mortensen",
        ///        "email": "kareah@mixalo.com",
        ///        "address": "SGFzc2VsYWdlciBBbGzDqSAxMHx8ODI2MCBWaWJ5IEo=",
        ///        "phone": "+4588888888"
        ///     }
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the body is null</response> 
        [HttpPost("api/users")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateUserCommand), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateUserDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            var command = new CreateUserCommand()
            {
                Name = body.Name,
                Email = body.Email,
                Address = body.Address,
                Phone = body.Phone
            };

            await _messageBus.SendAsync(command);

            return Created("api/users", command);
        }

        /// <summary>
        /// Updates a specifi user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/users/HYIlwOHnLR7cuUFY82Xj
        ///     {
        ///        "name": "Gudrun Mortensen",
        ///        "email": "kareah@mixalo.com",
        ///        "address": "SGFzc2VsYWdlciBBbGzDqSAxMHx8ODI2MCBWaWJ5IEo=",
        ///        "phone": "+4588888888"
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param> 
        /// <param name="body"></param>  
        /// <response code="204"></response>
        /// <response code="400">If the body is null</response> 
        [HttpPut("api/users/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            await _messageBus.SendAsync(new UpdateUserCommand()
            {
                Id = id,
                Name = body.Name,
                Email = body.Email,
                Address = body.Address,
                Phone = body.Phone
            });

            return NoContent();
        }

        private UserModel GetUserByIdOrThrowException(string id)
        {
            UserModel model = _unitOfWork.Users.GetByIdAsync(id).Result;

            if (model == null) { throw ExceptionFactory.UserWithIdNotFoundException(id); }

            return model;
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
