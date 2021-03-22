using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Mit_Oersted.Domain.Commands.Addresses;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Controllers;
using Mit_Oersted.WebApi.Models.Addresses;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Mit_Oersted.WebApi.Test.Controllers
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class AddressControllerTests
    {
        private IFixture _fixture;

        private readonly AddressController _sut;

        private readonly Mock<IMessageBus> _messageBusMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper<AddressModel, AddressDto>> _addressMapperMock = new();

        public AddressControllerTests()
        {
            _sut = new AddressController(
                _messageBusMock.Object,
                _unitOfWorkMock.Object,
                _addressMapperMock.Object
                );
        }

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void WHEN_GettingAllAddresses_GIVEN_AddressesInDb_THEN_ListOfAddressesIsReturned()
        {
            // Arrange
            var models = _fixture.Create<List<AddressModel>>();
            var dtos = new List<AddressDto>();

            foreach (AddressModel model in models)
            {
                dtos.Add(new AddressDto
                {
                    Id = model.Id,
                    AddressString = model.AddressString,
                    UserId = model.UserId
                });
            }

            _unitOfWorkMock.Setup(x => x.Addresses.GetAllAsync())
                .ReturnsAsync(models);

            for (int i = 0; i < models.Count; i++)
            {
                AddressModel model = models[i];
                AddressDto dto = dtos[i];

                _addressMapperMock.Setup(x => x.Map(model))
                    .Returns(dto);
            }

            // Act
            var actionResult = _sut.GetAllAddresses();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDtos = result.Value as List<AddressDto>;

            Assert.AreEqual(dtos.Count, resultDtos.Count);
            Assert.AreEqual(dtos[0].Id, resultDtos[0].Id);
            Assert.AreEqual(dtos[0].AddressString, resultDtos[0].AddressString);
        }

        [Test]
        public void WHEN_GettingAllAddresses_GIVEN_NoAddressesInDb_THEN_StringIsReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Addresses.GetAllAsync())
                .ReturnsAsync(new List<AddressModel>());

            // Act
            var actionResult = _sut.GetAllAddresses();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual("No users have been made yet", resultString);
        }

        [Test]
        public void WHEN_GettingAddress_GIVEN_AddressIdAndAddressesInDb_THEN_AddressIsReturned()
        {
            // Arrange
            var model = _fixture.Create<AddressModel>();
            var dto = new AddressDto
            {
                Id = model.Id,
                AddressString = model.AddressString,
                UserId = model.UserId
            };

            _unitOfWorkMock.Setup(x => x.Addresses.GetByIdAsync(model.Id))
                .ReturnsAsync(model);
            _addressMapperMock.Setup(x => x.Map(model))
                .Returns(dto);

            // Act
            var actionResult = _sut.GetAddress(model.Id);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDto = result.Value as AddressDto;

            Assert.AreEqual(dto.Id, resultDto.Id);
            Assert.AreEqual(dto.AddressString, resultDto.AddressString);
            Assert.AreEqual(dto.UserId, resultDto.UserId);
        }

        [Test]
        public void WHEN_GettingAddress_GIVEN_NoAddressId_THEN_AddressIsNotReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Addresses.GetByIdAsync(null))
                .ReturnsAsync(() => null);
            _addressMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetAddress(null); });
        }

        [Test]
        public void WHEN_GettingAddress_GIVEN_DiffrentAddressIdAndAddressesInDb_THEN_AddressIsNotReturned()
        {
            // Arrange
            var model1 = _fixture.Create<AddressModel>();
            var model2 = _fixture.Create<AddressModel>();

            _unitOfWorkMock.Setup(x => x.Addresses.GetByIdAsync(model1.Id))
                .ReturnsAsync(() => null);
            _addressMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetAddress(model2.Id); });
        }

        [Test]
        public void WHEN_CreatingAddress_GIVEN_Body_THEN_CreatedResponeIsReturned()
        {
            // Arrange
            var dto = _fixture.Create<CreateAddressDto>();
            var command = new CreateAddressCommand
            {
                AddressString = dto.AddressString,
                UserId = dto.UserId
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.CreateAddress(dto);

            // Assert
            var result = actionResult.Result as CreatedResult;
            var resultCommand = result.Value as CreateAddressCommand;

            Assert.AreEqual(command.AddressString, resultCommand.AddressString);
            Assert.AreEqual(command.UserId, resultCommand.UserId);
        }

        [Test]
        public void WHEN_CreatingAddress_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange

            // Act
            var actionResult = _sut.CreateAddress(null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Body was empty", resultString);
        }

        [Test]
        public void WHEN_UpdatingAddress_GIVEN_BodyAndAddressesInDb_THEN_NoContentResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dto = _fixture.Create<UpdateAddressDto>();
            var command = new UpdateAddressCommand
            {
                Id = id,
                AddressString = dto.AddressString,
                UserId = dto.UserId
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.UpdateAddress(id, dto);

            // Assert
            var result = actionResult.Result as NoContentResult;

            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public void WHEN_UpdatingAddress_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            var actionResult = _sut.UpdateAddress(id, null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Body was empty", resultString);
        }
    }
}
