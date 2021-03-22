using Mit_Oersted.Domain.Commands.Addresses;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
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

        public AddressCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            _ = _unitOfWork.Addresses.AddAsync(newDbModel).Result;
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
        }
    }
}
