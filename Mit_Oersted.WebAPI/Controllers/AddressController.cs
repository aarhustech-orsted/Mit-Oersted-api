using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mit_Oersted.Domain.Commands.Addresses;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mit_Oersted.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<AddressModel, AddressDto> _addressMapper;

        public AddressController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<AddressModel, AddressDto> addressMapper
            )
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _addressMapper = addressMapper ?? throw new ArgumentNullException(nameof(addressMapper));
        }

        /// <summary>
        /// Gets all addresses.
        /// </summary>
        /// <remarks> 
        /// Sample response:
        ///
        ///     GET /api/addresses
        ///     [
        ///        {
        ///            "id": "SGFzc2VsYWdlciBBbGzDqSAyIHx8IFZpYnkgSg==",
        ///            "userId": "HYIlwOHnLR7cuUFY82Xj",
        ///            "addressString": "Hasselager Allé 2, Viby J"
        ///        }
        ///     ]
        ///
        /// </remarks> 
        /// <returns>A list of addresses</returns> 
        [HttpGet("api/addresses")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetAllAddresses()
        {
            List<AddressModel> list = _unitOfWork.Addresses.GetAllAsync().Result;

            if (list.Count <= 0) { return Ok("No users have been made yet"); }

            return base.Ok((from AddressModel item in list
                            select _addressMapper.Map(item)).ToList());
        }

        /// <summary>
        /// Gets a specifi address.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/addresses/SGFzc2VsYWdlciBBbGzDqSAyIHx8IFZpYnkgSg==
        ///     {
        ///        "id": "SGFzc2VsYWdlciBBbGzDqSAyIHx8IFZpYnkgSg==",
        ///        "userId": "HYIlwOHnLR7cuUFY82Xj",
        ///        "addressString": "Hasselager Allé 2, Viby J"
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param>   
        /// <returns>A address by id</returns> 
        [HttpGet("api/addresses/{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetAddress(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw ExceptionFactory.UserWithIdNotFoundException(id); }

            return base.Ok(_addressMapper.Map(GetAddressByIdOrThrowException(id)));
        }

        /// <summary>
        /// Creates a new address.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/addresses
        ///     {
        ///        "userId": "fHG2h7ottdUFCsxyPesG",
        ///        "addressString": "Hasselager Allé 2, Viby J"
        ///     }
        ///     
        /// Sample response:
        ///
        ///     POST /api/addresses
        ///     {
        ///        "userId": "fHG2h7ottdUFCsxyPesG",
        ///        "addressString": "Hasselager Allé 2, Viby J"
        ///     }
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <returns>A newly created address</returns>
        /// <response code="201">Returns the newly created address</response>
        /// <response code="400">If the body is null</response> 
        [HttpPost("api/addresses")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateAddressCommand), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            var command = new CreateAddressCommand()
            {
                AddressString = body.AddressString,
                UserId = body.UserId
            };

            await _messageBus.SendAsync(command);

            return Created("api/addresses", command);
        }

        /// <summary>
        /// Updates a specifi address.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/addresses/SGFzc2VsYWdlciBBbGzDqSAyIHx8IFZpYnkgSg==
        ///     {
        ///        "userId": "EHmyg6pEdTaLkmWYE5NMb",
        ///        "addressString": "Hasselager Allé 2, Viby J"
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param> 
        /// <param name="body"></param>  
        /// <response code="204"></response>
        /// <response code="400">If the body is null</response> 
        [HttpPut("api/addresses/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(string id, [FromBody] UpdateAddressDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Body was empty");
            }

            await _messageBus.SendAsync(new UpdateAddressCommand()
            {
                Id = id,
                UserId = body.UserId,
                AddressString = body.AddressString
            });

            return NoContent();
        }

        private AddressModel GetAddressByIdOrThrowException(string id)
        {
            AddressModel model = _unitOfWork.Addresses.GetByIdAsync(id).Result;

            if (model == null) { throw ExceptionFactory.AddressNotFoundException(id); }

            return model;
        }
    }
}
