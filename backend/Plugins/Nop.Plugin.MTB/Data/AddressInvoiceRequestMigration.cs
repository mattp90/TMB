using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
{
    [NopMigration("2022-03-15 17:00:00", "InvoiceRequestAddressMigration")]
    public class InvoiceRequestAddressMigration : AutoReversingMigration
    {
        IMigrationManager _migrationManager;

        public InvoiceRequestAddressMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<InvoiceRequest>(Create);
            _migrationManager.BuildTable<InvoiceRequestAddress>(Create);
            _migrationManager.BuildTable<InvoiceRequestTransitCode>(Create);
        }
    }
}
