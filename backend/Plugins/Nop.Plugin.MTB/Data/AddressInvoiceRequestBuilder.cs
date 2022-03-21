using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
{
    internal class AddressInvoiceRequestBuilder : NopEntityBuilder<InvoiceRequestAddress>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InvoiceRequestAddress.InvoiceRequestId)).AsInt32().ForeignKey(nameof(InvoiceRequest), nameof(InvoiceRequest.Id)).OnDelete(System.Data.Rule.None).Indexed();
        }
    }
}
