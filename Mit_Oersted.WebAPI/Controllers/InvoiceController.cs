using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mit_Oersted.Domain.Commands.Invoices;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Models.Invoices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mit_Oersted.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<InvoiceModel, InvoiceDto> _invoiceMapper;

        public InvoiceController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<InvoiceModel, InvoiceDto> invoiceMapper
            )
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _invoiceMapper = invoiceMapper ?? throw new ArgumentNullException(nameof(invoiceMapper));
        }

        /// <summary>
        /// Gets all invoices.
        /// </summary>
        /// <remarks> 
        /// Sample response:
        ///
        ///     GET /api/invoices
        ///     [
        ///        {
        ///            "fileName", "U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM/dummy.rtf",
        ///            "downloadUrl", string
        ///        }
        ///     ]
        ///
        /// </remarks> 
        /// <returns>A list of invoices</returns> 
        [HttpGet("api/invoices")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetAllInvoicees()
        {
            List<InvoiceModel> list = _unitOfWork.Invoices.GetAllAsync().Result;

            if (list.Count <= 0) { return Ok("No invoices have been made yet"); }

            return base.Ok((from InvoiceModel item in list
                            select _invoiceMapper.Map(item)).ToList());
        }

        /// <summary>
        /// Gets all invoices in folder.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/invoices/U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM
        ///     [
        ///        {
        ///            "fileName", "U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM/dummy.rtf",
        ///            "downloadUrl", string
        ///        }
        ///     ]
        ///
        /// </remarks> 
        /// <param name="id"></param>   
        /// <returns>A invoice by id</returns> 
        [HttpGet("api/invoices/{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> GetInvoiceFolder(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw ExceptionFactory.InvoiceNotFoundException(id); }

            List<InvoiceModel> list = await GetInvoiceByFolderIdOrThrowException(id);

            if (list.Count <= 0) { return Ok("No invoices have been made yet"); }

            return base.Ok((from InvoiceModel item in list
                            select _invoiceMapper.Map(item)).ToList());
        }

        /// <summary>
        /// Gets a specifi invoice.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/invoices/U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM/dummy.rtf
        ///     {
        ///        "fileName": "U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM/dummy.rtf",
        ///        "downloadUrl": string
        ///     }
        ///
        /// </remarks> 
        /// <param name="folderId"></param>   
        /// <param name="fileId"></param> 
        /// <returns>A invoice by id</returns> 
        [HttpGet("api/invoices/{folderId}/{fileId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult GetInvoiceFile(string folderId, string fileId)
        {
            if (string.IsNullOrWhiteSpace(folderId) || string.IsNullOrWhiteSpace(fileId)) { throw ExceptionFactory.InvoiceNotFoundException($"{folderId}/{fileId}"); }

            return base.Ok(_invoiceMapper.Map(GetInvoiceByFolderIdAndFileIdOrThrowException($"{folderId}/{fileId}").Result));
        }

        /// <summary>
        /// Creates a new invoice.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/invoices
        ///     Content-type: multipart/form-data
        ///     Key: file, Value: the given file
        ///     Key: folderName, Value: U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM
        ///     Key: monthlyCost, Value: 0
        ///
        /// </remarks> 
        /// <param name="body"></param>
        /// <param name="file"></param>
        /// <returns>A newly created invoice</returns>
        /// <response code="200">Returns the newly created address</response>
        /// <response code="400">If the body is null</response> 
        [HttpPost("api/invoices")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateInvoice([FromForm] CreateInvoiceDto body, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null || file == null)
            {
                return BadRequest("Form was empty or fully filled out");
            }

            using Stream stream = file.OpenReadStream();

            byte[] result;
            using (var streamReader = new MemoryStream())
            {
                stream.CopyTo(streamReader);
                result = streamReader.ToArray();
            }

            await _messageBus.SendAsync(new CreateInvoiceCommand()
            {
                FolderName = body.FolderName,
                FileName = file.FileName,
                MetaData = new Dictionary<string, string>()
                {
                    { "monthlyCost", body.MonthlyCost }
                },
                File = result
            });

            return Ok("File have been added to storage");
        }

        /// <summary>
        /// Updates a specifi invoice.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/invoices/U3RyYW5kdmFuZ3N2ZWogMzcsIDgyNTAgRWeM
        ///     {
        ///        "fileName": "dummy.rtf",
        ///        "monthlyCost": 100
        ///     }
        ///
        /// </remarks> 
        /// <param name="id"></param> 
        /// <param name="body"></param>  
        /// <response code="204"></response>
        /// <response code="400">If the body is null</response> 
        [HttpPut("api/invoices/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateInvoice(string id, [FromBody] UpdateInvoiceDto body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (body == null)
            {
                return BadRequest("Form was empty");
            }

            await _messageBus.SendAsync(new UpdateInvoiceCommand()
            {
                FolderName = id,
                FileName = body.FileName,
                MetaData = new Dictionary<string, string>()
                {
                    { "monthlyCost", body.MonthlyCost.ToString() }
                }
            });

            return NoContent();
        }

        private async Task<InvoiceModel> GetInvoiceByFolderIdAndFileIdOrThrowException(string id)
        {
            InvoiceModel model = await _unitOfWork.Invoices.GetFileByIdAsync(id);

            if (model == null) { throw ExceptionFactory.InvoiceNotFoundException(id); }

            return model;
        }

        private async Task<List<InvoiceModel>> GetInvoiceByFolderIdOrThrowException(string id)
        {
            List<InvoiceModel> list = await _unitOfWork.Invoices.GetFolderByIdAsync(id);

            if (list == null) { throw ExceptionFactory.InvoiceNotFoundException(id); }

            return list;
        }
    }
}
