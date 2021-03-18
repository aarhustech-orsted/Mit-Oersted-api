using Microsoft.Extensions.Logging;
using Mit_Oersted.Domain.Commands.Invoices;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.ErrorHandling;
using Mit_Oersted.Domain.Events;
using Mit_Oersted.Domain.Events.Invoices;
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
        private readonly IEventStore _eventStore;
        private readonly InvoiceEventFactory _invoiceEventFactory;
        private readonly ILogger<InvoiceCommandHandler> _logger;

        public InvoiceCommandHandler(IUnitOfWork unitOfWork,
                                  IEventStore eventStore,
                                  InvoiceEventFactory invoiceEventFactory,
                                  ILogger<InvoiceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _invoiceEventFactory = invoiceEventFactory ?? throw new ArgumentNullException(nameof(invoiceEventFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Handle(CreateInvoiceCommand command)
        {
            if (command == null) { return; }
            if (_unitOfWork.Invoices.IsInvoiceAlreadyInUse($"{command.FolderName}/{command.FileName}")) { throw ExceptionFactory.InvoiceAlreadyExistException(); }

            string tmpId = _unitOfWork.Invoices.AddAsync($"{command.FolderName}/{command.FileName}", command.MetaData, command.File).Result;
            _eventStore.AddEvents(_invoiceEventFactory.GetUserCreatedEvent(tmpId));
        }

        public void Handle(UpdateInvoiceCommand command)
        {
            if (command == null) { return; }

            InvoiceModel model = _unitOfWork.Invoices.GetByIdAsync($"{command.FolderName}/{command.FileName}").Result;

            if (model == null) { throw ExceptionFactory.InvoiceNotFoundException($"{command.FolderName}/{command.FileName}"); }

            _unitOfWork.Invoices.UpdateAsync($"{command.FolderName}/{command.FileName}", command.MetaData);
            _eventStore.AddEvents(_invoiceEventFactory.GetUserUpdatedEvent($"{command.FolderName}/{command.FileName}"));
        }
    }
}
