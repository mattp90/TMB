using FluentMigrator;
using Nop.Core.Domain.Tasks;
using Nop.Data.Migrations;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Tasks;

namespace Nop.Plugin.TMB.Data
{
    [NopMigration("2022-03-23 15:00:00", "MigrationInvoiceRequest")]
    public class MigrationInvoiceRequest : AutoReversingMigration
    {
        IMigrationManager _migrationManager;

        public MigrationInvoiceRequest(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<InvoiceRequestState>(Create);
            _migrationManager.BuildTable<InvoiceRequestTransitCodeState>(Create);
            _migrationManager.BuildTable<InvoiceRequest>(Create);
            _migrationManager.BuildTable<InvoiceRequestAddress>(Create);
            _migrationManager.BuildTable<InvoiceRequestTransitCode>(Create);
            _migrationManager.BuildTable<InvoiceRequestFiscalId>(Create);
        }
    }
    
    [NopMigration("2022/03/29 16:00:00", "Invoice Request - Insert new data")]
    public class InvoiceRequestInsertDataMigration : AutoReversingMigration
    {
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly IScheduleTaskService _scheduleTaskService;
        
        public InvoiceRequestInsertDataMigration(IInvoiceRequestService invoiceRequestService, IScheduleTaskService scheduleTaskService)
        {
            _invoiceRequestService = invoiceRequestService;
            _scheduleTaskService = scheduleTaskService;
        }
        
        public override void Up()
        {
            var statePending = new InvoiceRequestState()
            {
                Description = "Pending"
            };
            _invoiceRequestService.InsertStateAsync(statePending);
            
            var stateFailed = new InvoiceRequestState()
            {
                Description = "Failed"
            };
            _invoiceRequestService.InsertStateAsync(stateFailed);
            
            var stateCompleted = new InvoiceRequestState()
            {
                Description = "Completed"
            };
            _invoiceRequestService.InsertStateAsync(stateCompleted);

            var transitionStateSuccess = new InvoiceRequestTransitCodeState()
            {
                Description = "Success"
            };
            _invoiceRequestService.InsertTransitionCodeStateAsync(transitionStateSuccess);
            
            var transitionStateFailure = new InvoiceRequestTransitCodeState()
            {
                Description = "Failure"
            };
            _invoiceRequestService.InsertTransitionCodeStateAsync(transitionStateFailure);
            
            var transitionStateCompleted = new InvoiceRequestTransitCodeState()
            {
                Description = "Completed"
            };
            _invoiceRequestService.InsertTransitionCodeStateAsync(transitionStateCompleted);

            var taskCheckInvoiceResponse =
                new ScheduleTask
                {
                    Name = "Check invoice request answers",
                    Seconds = 3600,
                    Type = "Nop.Plugin.TMB.Task.CheckInvoiceResponseTask, Nop.Plugin.TMB",
                    Enabled = true,
                    StopOnError = false
                };
            _scheduleTaskService.InsertTaskAsync(taskCheckInvoiceResponse);
        }
    }
}
