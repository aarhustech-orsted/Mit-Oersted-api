using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mit_Oersted.Domain.Commands.Users;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.WebApi.Controllers;
using Mit_Oersted.WebApi.Models.Tokens;
using Mit_Oersted.WebApi.Models.Users;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Mit_Oersted.WebApi.Test.Controllers
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class UserControllerTests
    {
        private IFixture _fixture;

        private readonly UserController _sut;

        private readonly Mock<IMessageBus> _messageBusMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper<UserModel, UserDto>> _userMapperMock = new();
        private readonly Mock<IMapper<RefreshTokenResponseDto, TokenResponseBodyDto>> _tokenRefreshMapperMock = new();
        private readonly Mock<IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto>> _signInWithPhoneNumberMapperMock = new();
        private readonly Mock<IConfiguration> _configMock = new();

        public UserControllerTests()
        {
            _sut = new UserController(
                _messageBusMock.Object,
                _unitOfWorkMock.Object,
                _userMapperMock.Object,
                _tokenRefreshMapperMock.Object,
                _signInWithPhoneNumberMapperMock.Object,
                _configMock.Object
                );
        }

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void WHEN_GettingAllUsers_GIVEN_UsersInDb_THEN_ListOfUsersIsReturned()
        {
            // Arrange
            var models = _fixture.Create<List<UserModel>>();
            var dtos = new List<UserDto>();

            foreach (UserModel model in models)
            {
                dtos.Add(new UserDto
                {
                    Id = model.Id,
                    Address = model.Address,
                    Email = model.Email,
                    Name = model.Name,
                    Phone = model.Phone
                });
            }

            _unitOfWorkMock.Setup(x => x.Users.GetAllAsync())
                .ReturnsAsync(models);

            for (int i = 0; i < models.Count; i++)
            {
                UserModel model = models[i];
                UserDto dto = dtos[i];

                _userMapperMock.Setup(x => x.Map(model))
                    .Returns(dto);
            }

            // Act
            var actionResult = _sut.GetAllUsers();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDtos = result.Value as List<UserDto>;

            Assert.AreEqual(dtos.Count, resultDtos.Count);
            Assert.AreEqual(dtos[0].Id, resultDtos[0].Id);
            Assert.AreEqual(dtos[0].Name, resultDtos[0].Name);
            Assert.AreEqual(dtos[0].Address, resultDtos[0].Address);
        }

        [Test]
        public void WHEN_GettingAllUsers_GIVEN_NoUsersInDb_THEN_StringIsReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Users.GetAllAsync())
                .ReturnsAsync(new List<UserModel>());

            // Act
            var actionResult = _sut.GetAllUsers();

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual("No users have been made yet", resultString);
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_UserIdAndUsersInDb_THEN_UserIsReturned()
        {
            // Arrange
            var model = _fixture.Create<UserModel>();
            var dto = new UserDto
            {
                Id = model.Id,
                Address = model.Address,
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone
            };

            _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(model.Id))
                .ReturnsAsync(model);
            _userMapperMock.Setup(x => x.Map(model))
                .Returns(dto);

            // Act
            var actionResult = _sut.GetUser(model.Id);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            var resultDto = result.Value as UserDto;

            Assert.AreEqual(dto.Id, resultDto.Id);
            Assert.AreEqual(dto.Name, resultDto.Name);
            Assert.AreEqual(dto.Address, resultDto.Address);
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_NoUserId_THEN_UserIsNotReturned()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(null))
                .ReturnsAsync(() => null);
            _userMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetUser(null); });
        }

        [Test]
        public void WHEN_GettingUser_GIVEN_DiffrentUserIdAndUsersInDb_THEN_UserIsNotReturned()
        {
            // Arrange
            var model1 = _fixture.Create<UserModel>();
            var model2 = _fixture.Create<UserModel>();

            _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(model1.Id))
                .ReturnsAsync(() => null);
            _userMapperMock.Setup(x => x.Map(null))
                .Returns(() => null);

            // Act

            // Assert
            Assert.Throws<DomainException>(() => { _sut.GetUser(model2.Id); });
        }

        [Test]
        public void WHEN_CreatingUser_GIVEN_Body_THEN_CreatedResponeIsReturned()
        {
            // Arrange
            var dto = _fixture.Create<CreateUserDto>();
            var command = new CreateUserCommand
            {
                Name = dto.Name,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.CreateUser(dto);

            // Assert
            var result = actionResult.Result as CreatedResult;
            var resultCommand = result.Value as CreateUserCommand;

            Assert.AreEqual(command.Name, resultCommand.Name);
            Assert.AreEqual(command.Address, resultCommand.Address);
        }

        [Test]
        public void WHEN_CreatingUser_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange

            // Act
            var actionResult = _sut.CreateUser(null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Body was empty", resultString);
        }

        [Test]
        public void WHEN_UpdatingUser_GIVEN_BodyAndUsersInDb_THEN_NoContentResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dto = _fixture.Create<UpdateUserDto>();
            var command = new UpdateUserCommand
            {
                Id = id,
                Name = dto.Name,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _messageBusMock.Setup(x => x.SendAsync(command))
                .Returns(() => null);

            // Act
            var actionResult = _sut.UpdateUser(id, dto);

            // Assert
            var result = actionResult.Result as NoContentResult;

            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public void WHEN_UpdatingUser_GIVEN_NoBody_THEN_BadRequestResponeIsReturned()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            var actionResult = _sut.UpdateUser(id, null);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            var resultString = result.Value as string;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Body was empty", resultString);
        }
    }
}
