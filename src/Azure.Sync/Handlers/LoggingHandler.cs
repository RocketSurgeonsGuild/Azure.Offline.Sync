using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;

namespace Rocket.Surgery.Azure.Sync.Handlers
{
    public class LoggingHandler : MobileServiceSyncHandler
    {
        public LoggingHandler()
        {

        }

        /// <inheritdoc />
        public override Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            Debug.WriteLine("Pushed to server - {0} - {1} errors", result.Status, result.Errors);
            return base.OnPushCompleteAsync(result);
        }

        /// <inheritdoc />
        public override Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            Debug.WriteLine("About to execute table {0} operation of {1}", operation.Table.TableName, operation.Kind);

            return base.ExecuteTableOperationAsync(operation);
        }
    }
}
