using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.TMB.Services.InvoiceRequest;

namespace Nop.Plugin.TMB.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1000;

        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            // MVC Helpers
            services.AddScoped<IInvoiceRequestService, InvoiceRequestService>();
        }
    }
}
