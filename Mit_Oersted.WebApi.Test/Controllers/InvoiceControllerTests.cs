using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mit_Oersted.Domain.Commands.Invoices;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Controllers;
using Mit_Oersted.WebApi.Models.Invoices;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mit_Oersted.WebApi.Test.Controllers
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class InvoiceControllerTests
    {
        private IFixture _fixture;

        private readonly InvoiceController _sut;

        private readonly Mock<IMessageBus> _messageBusMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper<InvoiceModel, InvoiceDto>> _invoiceMapperMock = new();

        public InvoiceControllerTests()
        {
            _sut = new InvoiceController(
                _messageBusMock.Object,
                _unitOfWorkMock.Object,
                _invoiceMapperMock.Object
                );
        }

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void WHEN_GettingAllInvoices_GIVEN_InvoicesInStorage_THEN_ListOfInvoicesIsReturned()
        {
            // Arrange
            var models = _fixture.Create<List<InvoiceModel>>();
            var dtos = new List<InvoiceDto>();

            foreach (InvoiceModel model in models)
            {
                dtos.Add(new InvoiceDto
                {
                    DownloadUrl = model.DownloadUrl,
                    FileName = model.Name,
                    MetaData = model.Metadata
                });
            }

            _unitOfWorkMock.Setup(x => x.Invoices.GetAllAsync())
                .ReturnsAsync(models);

            for (int i = 0; i < models.Count; i++)
            {
                InvoiceModel model = models[i];
                InvoiceDto dto = dtos[i];

                _invoiceMapperMock.Setup(x => x.Map(model))
                    .Returns(dto);
            }

            // Act
            var actionResult = _sut.GetAllInvoices();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDtos = result.Value as List<InvoiceDto>;

            Assert.AreEqual(dtos.Count, resultDtos.Count);
            Assert.AreEqual(dtos[0].FileName, resultDtos[0].FileName);
            Assert.AreEqual(dtos[0].DownloadUrl, resultDtos[0].DownloadUrl);
            Assert.AreEqual(dtos[0].MetaData, resultDtos[0].MetaData);
        }

        [Test]
        public void WHEN_GettingAllUsers_GIVEN_NoInvoicesInStorage_THEN_StringIsReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Invoices.GetAllAsync())
                .ReturnsAsync(new List<InvoiceModel>());

            // Act
            var actionResult = _sut.GetAllInvoices();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual("No invoices have been made yet", resultString);
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_InvoiceIdAndInvoicesInStorage_THEN_InvoiceIsReturned()
        {
            // Arrange
            var fileName = Guid.NewGuid().ToString();
            var folderName = Guid.NewGuid().ToString();
            var model = _fixture.Create<InvoiceModel>();
            var dto = new InvoiceDto
            {
                DownloadUrl = model.DownloadUrl,
                FileName = model.Name,
                MetaData = model.Metadata
            };

            _unitOfWorkMock.Setup(x => x.Invoices.GetFileByIdAsync($"{folderName}/{fileName}"))
                .ReturnsAsync(model);
            _invoiceMapperMock.Setup(x => x.Map(model))
                .Returns(dto);

            // Act
            var actionResult = _sut.GetInvoiceFile(folderName, fileName);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDto = result.Value as InvoiceDto;

            Assert.AreEqual(dto.DownloadUrl, resultDto.DownloadUrl);
            Assert.AreEqual(dto.FileName, resultDto.FileName);
            Assert.AreEqual(dto.MetaData, resultDto.MetaData);
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_NoInvoiceId_THEN_InvoiceIsNotReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(null))
                .ReturnsAsync(() => null);

            _invoiceMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetInvoiceFile(null, null); });
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_DiffrentInvoiceIdAndInvoicesInStorage_THEN_InvoiceIsNotReturned()
        {
            // Arrange
            var model1 = _fixture.Create<InvoiceModel>();
            var fileName1 = Guid.NewGuid().ToString();
            var folderName1 = Guid.NewGuid().ToString();

            var model2 = _fixture.Create<InvoiceModel>();
            var fileName2 = Guid.NewGuid().ToString();
            var folderName2 = Guid.NewGuid().ToString();

            _unitOfWorkMock.Setup(x => x.Invoices.GetFileByIdAsync($"{folderName1}/{fileName1}"))
                .ReturnsAsync(() => null);

            _invoiceMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetInvoiceFile(folderName2, fileName2); });
        }

        [Test]
        public void WHEN_CreatingInvoice_GIVEN_Body_THEN_CreatedResponeIsReturned()
        {
            // Arrange
            var dto = _fixture.Create<CreateInvoiceDto>();
            var file = CreateTestFormFile(_fixture.Create<string>(), _fixture.Create<string>());
            var metaData = new Dictionary<string, string>()
            {
                { "monthlyCost", dto.MonthlyCost }
            };

            using Stream stream = file.OpenReadStream();

            byte[] resultBytes;
            using (var streamReader = new MemoryStream())
            {
                stream.CopyTo(streamReader);
                resultBytes = streamReader.ToArray();
            }

            var command = new CreateInvoiceCommand
            {
                FileName = file.FileName,
                FolderName = dto.FolderName,
                File = resultBytes,
                MetaData = metaData
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.CreateInvoice(dto, file);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("File have been added to storage", resultString);
        }

        [Test]
        public void WHEN_CreatingInvoice_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange

            // Act
            var actionResult = _sut.CreateInvoice(null, null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Form was empty or fully filled out", resultString);
        }

        [Test]
        public void WHEN_UpdatingInvoice_GIVEN_BodyAndInvoicesInStorage_THEN_NoContentResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dto = _fixture.Create<UpdateInvoiceDto>();
            var metaData = new Dictionary<string, string>()
            {
                { "monthlyCost", dto.MonthlyCost.ToString() }
            };
            var command = new UpdateInvoiceCommand
            {
                FolderName = id,
                FileName = dto.FileName,
                MetaData = metaData
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.UpdateInvoice(id, dto);

            // Assert
            var result = actionResult.Result as NoContentResult;

            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public void WHEN_UpdatingInvoice_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            var actionResult = _sut.UpdateInvoice(id, null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Body was empty", resultString);
        }

        private static IFormFile CreateTestFormFile(string p_Name, string p_Content)
        {
            byte[] s_Bytes = Encoding.UTF8.GetBytes(p_Content);

            return new FormFile(
                baseStream: new MemoryStream(s_Bytes),
                baseStreamOffset: 0,
                length: s_Bytes.Length,
                name: "Data",
                fileName: p_Name
            );
        }
    }
}
