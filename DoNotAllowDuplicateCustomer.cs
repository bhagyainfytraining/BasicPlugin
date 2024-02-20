using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BasicPlugin
{
    public class DoNotAllowDuplicateCustomer : IPlugin
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
                Entity potentialCustomer = (Entity)context.InputParameters["Target"];

                tracingService.Trace("DoNotAllowDuplicateCustomer: Inside The Context");

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    string phoneNumber = potentialCustomer.GetAttributeValue<string>("tt1_customerphonenumber");

                    string pancard = potentialCustomer.GetAttributeValue<string>("tt1_pancard");

                    string email = potentialCustomer.GetAttributeValue<string>("tt1_customeremail");
                    tracingService.Trace("DoNotAllowDuplicateCustomer: Inside The try " + phoneNumber);
                    tracingService.Trace("DoNotAllowDuplicateCustomer: Inside The try " + pancard);
                    tracingService.Trace("DoNotAllowDuplicateCustomer: Inside The try " + email);


                    QueryExpression fetchCustomers = new QueryExpression("tt1_potentialcustomer");
                    fetchCustomers.ColumnSet = new ColumnSet("tt1_customerphonenumber", "tt1_pancard", "tt1_customeremail");
                    if (!string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        fetchCustomers.Criteria.AddCondition(new ConditionExpression("tt1_customerphonenumber", ConditionOperator.Equal, phoneNumber));
                    }
                    if (!string.IsNullOrWhiteSpace(pancard))
                    {
                        fetchCustomers.Criteria.AddCondition(new ConditionExpression("tt1_pancard", ConditionOperator.Equal, pancard));
                    }
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        fetchCustomers.Criteria.AddCondition(new ConditionExpression("tt1_customeremail", ConditionOperator.Equal, email));
                    }
                
                    FilterExpression filter = new FilterExpression(LogicalOperator.Or);
                    fetchCustomers.Criteria.AddFilter(filter);
                    EntityCollection potentailCustomersRecords = service.RetrieveMultiple(fetchCustomers);
                    tracingService.Trace("DoNotAllowDuplicateCustomer: Inside The try " + potentailCustomersRecords);

                    if (potentailCustomersRecords.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("Already a Duplicate potentail customer exists with the same values of  Email, Phone number and Pan Card");
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }
            }
        }
    }
}
