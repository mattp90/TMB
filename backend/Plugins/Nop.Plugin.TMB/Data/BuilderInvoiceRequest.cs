using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.TMB.Entity;

namespace Nop.Plugin.TMB.Data
{
    internal class BuilderInvoiceRequest : NopEntityBuilder<InvoiceRequest>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InvoiceRequest.InvoiceRequestStatusId))
                .AsInt32()
                .Nullable()
                .ForeignKey(nameof(InvoiceRequestStatus), nameof(InvoiceRequestStatus.Id))
                .OnDelete(System.Data.Rule.None)
                .Indexed();
        }
    }
}
