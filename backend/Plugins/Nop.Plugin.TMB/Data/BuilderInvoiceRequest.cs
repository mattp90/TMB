using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.TMB.Entity;

namespace Nop.Plugin.TMB.Data
{
    internal class BuilderInvoiceRequest : NopEntityBuilder<InvoiceRequest>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InvoiceRequest.InvoiceRequestStateId))
                .AsInt32()
                .Nullable()
                .ForeignKey(nameof(InvoiceRequestState), nameof(InvoiceRequestState.Id))
                .OnDelete(System.Data.Rule.None)
                .Indexed();
        }
    }
}
