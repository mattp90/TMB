using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.TMB.Task
{
    public partial class CheckInvoiceResponseTask : IScheduleTask
    {
        private readonly ILogger _logger;
        
        public CheckInvoiceResponseTask(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.InformationAsync("Execution test");
        }
    }
}