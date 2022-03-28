using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;

namespace Nop.Plugin.TMB
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            if (context.ActionContext.ActionDescriptor.DisplayName != null && !string.IsNullOrEmpty(context.AreaName) &&
                context.AreaName.Equals(AreaNames.Admin) &&
                !context.ActionContext.ActionDescriptor.DisplayName.Contains("TMB"))
            {
                return viewLocations;
            }

            viewLocations = new[] { $"/Plugins/{TMB.PLUGIN_NAME_SPACE}/Views/{{0}}.cshtml" }.Concat(viewLocations);
            viewLocations = new[] { $"/Plugins/{TMB.PLUGIN_NAME_SPACE}/Views/{context.ControllerName}/{{0}}.cshtml" }.Concat(viewLocations);

            viewLocations = new[] { $"~/Views/{context.ViewName}.cshtml" }.Concat(viewLocations);
            viewLocations = new[] { $"~/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);

            return viewLocations;
        }
    }
}
