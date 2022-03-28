using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.TMB.Entity;

namespace Nop.Plugin.TMB.Data
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
