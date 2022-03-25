using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
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
