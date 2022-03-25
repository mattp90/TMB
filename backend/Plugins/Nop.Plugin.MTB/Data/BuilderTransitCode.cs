using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Data
{
    internal class BuilderTransitCode : NopEntityBuilder<InvoiceRequestTransitCode>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(InvoiceRequestTransitCode.InvoiceRequestId))
                .AsInt32()
                .ForeignKey(nameof(InvoiceRequest), nameof(InvoiceRequest.Id))
                .OnDelete(System.Data.Rule.None)
                .Indexed();
            
            table.WithColumn(nameof(InvoiceRequestTransitCode.InvoiceRequestTransitCodeStateId))
                .AsInt32()
                .Nullable()
                .ForeignKey(nameof(InvoiceRequestTransitCodeState), nameof(InvoiceRequestTransitCodeState.Id))
                .OnDelete(System.Data.Rule.None)
                .Indexed();
        }
    }
}
