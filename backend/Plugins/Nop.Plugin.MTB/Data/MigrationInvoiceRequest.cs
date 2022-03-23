using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
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
        }
    }
}
