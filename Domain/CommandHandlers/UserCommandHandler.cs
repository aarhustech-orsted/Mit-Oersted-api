using Mit_Oersted.Domain.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Mit_Oersted.Domain.Events;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.Domain.Commands;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Events.User;

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

            var newDbModel = new User()
            {
                Name = command.Name,
                Address = command.Address,
                Email = command.Email,
                Phone = command.Phone
            };

            newDbModel.Id = _unitOfWork.Users.Add(newDbModel).Result;

            _eventStore.AddEvents(_userEventFactory.GetUserCreatedEvent(newDbModel));
        }

        public void Handle(UpdateUserCommand command)
        {
            User user = _unitOfWork.Users.GetByIdAsync(command.Id).Result;

            if (user == null) { return; }

            _unitOfWork.Users.Update(user.Id, new Dictionary<string, object>()
            {
                {  "name", command.Name ?? user.Name },
                {  "email", command.Email ?? user.Email },
                {  "phone", command.Phone ?? user.Phone },
                {  "address", command.Address ?? user.Address }
            });

            _eventStore.AddEvents(_userEventFactory.GetUserUpdatedEvent(user));
        }
    }
}
