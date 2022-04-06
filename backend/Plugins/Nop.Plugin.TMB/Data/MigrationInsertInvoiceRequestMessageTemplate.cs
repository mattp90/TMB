using System;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tasks;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Messages;
using Nop.Services.Tasks;
using System.Linq;

namespace Nop.Plugin.TMB.Data
{
    [NopMigration("2022/03/31 13:00:00", "Invoice Request - Insert new message template")]
    public class MigrationInsertInvoiceRequestMessageTemplate : AutoReversingMigration
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private IRepository<EmailAccount> _emailAccountRepository;

        public MigrationInsertInvoiceRequestMessageTemplate(IInvoiceRequestService invoiceRequestService, IScheduleTaskService scheduleTaskService, IMessageTemplateService messageTemplateService, IEmailAccountService emailAccountService, IRepository<EmailAccount> emailAccountRepository)
        {
            _messageTemplateService = messageTemplateService;
            _emailAccountRepository = emailAccountRepository;
        }
        
        public override void Up()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            
            var messateTemplate = new MessageTemplate
            {
                Name = MessageTemplateSystemNames.InvoiceResponseMessage,
                Subject = "Fattura per il transito %InvoiceRequestTransitCode.Code%",
                Body =
                    $"<p>Gentile Cliente,<br />{Environment.NewLine}" +
                    $"in allegato può trovare la fattura per il transito con identificativo <strong>\"%InvoiceRequestTransitCode.Code%\"</strong>.<br />{Environment.NewLine}" +
                    $"Grazie." +
                    $"</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            };

            _messageTemplateService.InsertMessageTemplateAsync(messateTemplate);
        }
    }
}