using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.TMB.Entity;

namespace Nop.Plugin.TMB.Data
{
    internal class BuilderInvoiceRequestFiscalId : NopEntityBuilder<InvoiceRequestFiscalId>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InvoiceRequestFiscalId.InvoiceRequestId))
                .AsInt32()
                .ForeignKey(nameof(InvoiceRequest), nameof(InvoiceRequest.Id))
                .OnDelete(System.Data.Rule.None)
                .Indexed();
        }
    }
}
