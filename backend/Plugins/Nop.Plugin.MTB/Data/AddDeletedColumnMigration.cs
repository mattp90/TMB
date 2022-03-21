using FluentMigrator;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
{
    [NopMigration("2022-03-16 16:30:00", "AddDeletedColumnMigration")]
    public class AddDeletedColumnMigration : AutoReversingMigration
    {
        public override void Up()
        {
            if(!Schema.Table(NameCompatibilityManager.GetTableName(typeof(InvoiceRequest))).Column(nameof(InvoiceRequest.Deleted)).Exists())
                Alter.Table(nameof(InvoiceRequest))
                    .AddColumn(nameof(InvoiceRequest.Deleted))
                    .AsBoolean()
                    .NotNullable()
                    .WithDefaultValue(false);
        }
    }
}
