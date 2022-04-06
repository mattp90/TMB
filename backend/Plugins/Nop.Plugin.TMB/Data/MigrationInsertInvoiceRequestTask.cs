using FluentMigrator;
using FluentMigrator.Infrastructure;
using Nop.Core.Domain.Tasks;
using Nop.Data.Migrations;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Tasks;

namespace Nop.Plugin.TMB.Data
{
    [NopMigration("2022/03/31 10:00:00", "Invoice Request - Insert new task")]
    public class MigrationInsertInvoiceRequestTask : AutoReversingMigration
    {
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly IScheduleTaskService _scheduleTaskService;
        
        public MigrationInsertInvoiceRequestTask(IInvoiceRequestService invoiceRequestService, IScheduleTaskService scheduleTaskService)
        {
            _invoiceRequestService = invoiceRequestService;
            _scheduleTaskService = scheduleTaskService;
        }
        
        public override void Up()
        {
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