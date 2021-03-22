using Mit_Oersted.Domain.Commands.Invoices;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Repository;
using System;

namespace Mit_Oersted.Domain.CommandHandler
{
    public class InvoiceCommandHandler :
        ICommandHandler,
        ICommandHandler<CreateInvoiceCommand>,
        ICommandHandler<UpdateInvoiceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void Handle(CreateInvoiceCommand command)
        {
            if (command == null) { return; }
            if (_unitOfWork.Invoices.IsInvoiceAlreadyInUse($"{command.FolderName}/{command.FileName}")) { throw ExceptionFactory.InvoiceAlreadyExistException(); }

            _ = _unitOfWork.Invoices.AddAsync($"{command.FolderName}/{command.FileName}", command.MetaData, command.File).Result;
        }

        public void Handle(UpdateInvoiceCommand command)
        {
            if (command == null) { return; }

            InvoiceModel model = _unitOfWork.Invoices.GetFileByIdAsync($"{command.FolderName}/{command.FileName}").Result;

            if (model == null) { throw ExceptionFactory.InvoiceFileInFolderNotFoundException(command.FolderName, command.FileName); }

            _unitOfWork.Invoices.UpdateAsync($"{command.FolderName}/{command.FileName}", command.MetaData);
        }
    }
}
