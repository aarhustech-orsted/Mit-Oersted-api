using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Commands.Users;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Events;
using Mit_Oersted.Domain.Events.Users;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using System;
using System.Collections.Generic;

namespace Mit_Oersted.Domain.CommandHandler
{
    public class UserCommandHandler :
        ICommandHandler,
        ICommandHandler<CreateUserCommand>,
        ICommandHandler<UpdateUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventStore _eventStore;
        private readonly UserEventFactory _userEventFactory;
        private readonly ILogger<UserCommandHandler> _logger;

        public UserCommandHandler(IUnitOfWork unitOfWork,
                                  IEventStore eventStore,
                                  UserEventFactory userEventFactory,
                                  ILogger<UserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _userEventFactory = userEventFactory ?? throw new ArgumentNullException(nameof(userEventFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Handle(CreateUserCommand command)
        {
            if (command == null) { return; }

            if (_unitOfWork.Users.IsEmailAlreadyInUse(command.Email)) { throw ExceptionFactory.UserWithEmailAlreadyExistException(command.Email); }

            var newDbModel = new UserModel()
            {
                Name = command.Name,
                Address = command.Address,
                Email = command.Email,
                Phone = command.Phone
            };

            newDbModel.Id = _unitOfWork.Users.AddAsync(newDbModel).Result;

            _eventStore.AddEvents(_userEventFactory.GetUserCreatedEvent(newDbModel));
        }

        public void Handle(UpdateUserCommand command)
        {
            if (command == null) { return; }

            UserModel user = _unitOfWork.Users.GetByIdAsync(command.Id).Result;

            if (user == null) { throw ExceptionFactory.UserNotFoundException(command.Id); }

            var dataToUpdate = new Dictionary<string, object>();

            if (user.Name != command.Name && !string.IsNullOrEmpty(command.Name))
            {
                dataToUpdate.Add("name", command.Name);
            }

            if (user.Email != command.Email && !string.IsNullOrEmpty(command.Email))
            {
                dataToUpdate.Add("email", command.Email);
            }

            if (user.Phone != command.Phone && !string.IsNullOrEmpty(command.Phone))
            {
                dataToUpdate.Add("phone", command.Phone);
            }

            if (user.Address != command.Address && !string.IsNullOrEmpty(command.Address))
            {
                dataToUpdate.Add("address", command.Address);
            }

            if (dataToUpdate.Count <= 0)
            {
                return;
            }

            _unitOfWork.Users.UpdateAsync(user.Id, dataToUpdate);

            _eventStore.AddEvents(_userEventFactory.GetUserUpdatedEvent(user));
        }
    }
}
