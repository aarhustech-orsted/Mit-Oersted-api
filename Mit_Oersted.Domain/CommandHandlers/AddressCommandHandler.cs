using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Commands.Addresses;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Events;
using Mit_Oersted.Domain.Events.Addresses;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using System;
using System.Collections.Generic;

namespace Mit_Oersted.Domain.CommandHandler
{
    public class AddressCommandHandler :
        ICommandHandler,
        ICommandHandler<CreateAddressCommand>,
        ICommandHandler<UpdateAddressCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventStore _eventStore;
        private readonly AddressEventFactory _addressEventFactory;
        private readonly ILogger<AddressCommandHandler> _logger;

        public AddressCommandHandler(IUnitOfWork unitOfWork,
                                  IEventStore eventStore,
                                  AddressEventFactory addressEventFactory,
                                  ILogger<AddressCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _addressEventFactory = addressEventFactory ?? throw new ArgumentNullException(nameof(addressEventFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Handle(CreateAddressCommand command)
        {
            if (command == null) { return; }

            if (_unitOfWork.Addresses.IsAddressAlreadyInUse(command.AddressString)) { throw ExceptionFactory.AddressAlreadyExistException(); }

            string[] AddressStringSplit = command.AddressString.Split(',');
            string outString = null;

            for (int i = 0; i < AddressStringSplit.Length; i++)
            {
                string substring = AddressStringSplit[i];
                if (i == 0)
                {
                    outString += $"{substring.Trim()} |";
                }
                else
                {

                    outString += $"| {substring.Trim()}";
                }
            }

            var newDbModel = new AddressModel()
            {
                Id = _unitOfWork.Addresses.Base64Encode(outString),
                UserId = command.UserId,
                AddressString = command.AddressString
            };

            newDbModel.Id = _unitOfWork.Addresses.AddAsync(newDbModel).Result;

            _eventStore.AddEvents(_addressEventFactory.GetAddressCreatedEvent(newDbModel));
        }

        public void Handle(UpdateAddressCommand command)
        {
            if (command == null) { return; }

            AddressModel model = _unitOfWork.Addresses.GetByIdAsync(command.Id).Result;

            if (model == null) { throw ExceptionFactory.AddressNotFoundException(command.Id); }

            var dataToUpdate = new Dictionary<string, object>();

            if (model.UserId != command.UserId && !string.IsNullOrEmpty(command.UserId))
            {
                dataToUpdate.Add("userId", command.UserId);
            }

            if (model.AddressString != command.AddressString && !string.IsNullOrEmpty(command.AddressString))
            {
                dataToUpdate.Add("addressString", command.AddressString);
            }

            if (dataToUpdate.Count <= 0)
            {
                return;
            }

            _unitOfWork.Addresses.UpdateAsync(model.Id, dataToUpdate);

            _eventStore.AddEvents(_addressEventFactory.GetAddressUpdatedEvent(model));
        }
    }
}
