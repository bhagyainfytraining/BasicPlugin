using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BasicPlugin
{
    public class CheckAccount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
            serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName == "account")
                {
                    Entity account = (Entity)context.InputParameters["Target"];
                    Entity preImageAccount = (Entity)context.PreEntityImages["Image"];
                    Entity postImageAccount = (Entity)context.PostEntityImages["Image"];

                    string preImagePhoneNumber = preImageAccount.GetAttributeValue<string>("telephone1");
                    string postImagePhoneNumber = postImageAccount.GetAttributeValue<string>("telephone1");

                    tracingService.Trace("Pre-image phone number: {0}, Post-image phone number: {1}", preImagePhoneNumber, postImagePhoneNumber);
                }
            }
        }
    }
}
