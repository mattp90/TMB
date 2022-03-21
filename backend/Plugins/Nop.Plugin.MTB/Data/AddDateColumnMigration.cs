using FluentMigrator;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
{
    [NopMigration("2022-03-16 16:00:00", "AddDatemigration")]
    public class UpdateNopMigration : AutoReversingMigration
    {
        public override void Up()
        {
            if(!Schema.Table(NameCompatibilityManager.GetTableName(typeof(InvoiceRequestAddress))).Column(nameof(InvoiceRequestAddress.CreatedOnUTC)).Exists())
                Alter.Table(nameof(InvoiceRequestAddress))
                    .AddColumn(nameof(InvoiceRequestAddress.CreatedOnUTC))
                    .AsDateTime()
                    .NotNullable();
            
            if(!Schema.Table(NameCompatibilityManager.GetTableName(typeof(InvoiceRequestAddress))).Column(nameof(InvoiceRequestAddress.UpdatedOnUTC)).Exists())
                Alter.Table(nameof(InvoiceRequestAddress))
                    .AddColumn(nameof(InvoiceRequestAddress.UpdatedOnUTC))
                    .AsDateTime()
                    .NotNullable();
            
            if(!Schema.Table(NameCompatibilityManager.GetTableName(typeof(InvoiceRequestTransitCode))).Column(nameof(InvoiceRequestTransitCode.CreatedOnUTC)).Exists())
                Alter.Table(nameof(InvoiceRequestTransitCode))
                    .AddColumn(nameof(InvoiceRequestTransitCode.CreatedOnUTC))
                    .AsDateTime()
                    .NotNullable();
            
            if(!Schema.Table(NameCompatibilityManager.GetTableName(typeof(InvoiceRequestTransitCode))).Column(nameof(InvoiceRequestTransitCode.UpdatedOnUTC)).Exists())
                Alter.Table(nameof(InvoiceRequestTransitCode))
                    .AddColumn(nameof(InvoiceRequestTransitCode.UpdatedOnUTC))
                    .AsDateTime()
                    .NotNullable();
        }
    }
}
