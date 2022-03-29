using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.TMB.Helpers
{
    public static class PermissionHelper
    {
        public static readonly PermissionRecord AccessInvoiceRequest = StandardPermissionProvider.AccessAdminPanel;
    }
}
