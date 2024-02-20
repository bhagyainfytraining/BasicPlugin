using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace BasicPlugin
{
    public class creatingBankAccOnCustomerCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity customerEntity = (Entity)context.InputParameters["Target"];

                tracingService.Trace("FollowUpPlugin: Inside The Context");

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    tracingService.Trace("creatingBankAccOnCustomerCreation: inside to Try");

                    Entity bankAcount = new Entity("tt1_bankaccount");

                    bankAcount["tt1_name"] = customerEntity.GetAttributeValue<string>("tt1_name");
                    bankAcount["tt1_accountcreatedondate"] = DateTime.Now;
                    bankAcount["tt1_kyccompleted"] = true;
                    bankAcount["tt1_accounttype"] = new OptionSetValue(958510002);
                    bankAcount["tt1_accountbalance"] = 10000;

                    service.Create(bankAcount);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

            }
        }
    }
}
