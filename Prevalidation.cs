using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.ServiceModel;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Crm.Sdk;

namespace BasicPlugin

{
    public class Prevalidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService.Trace("Called Associate Plugin");
            string relationship;
            EntityReference TargetEntity = null;
            EntityReferenceCollection RelatedEntityCollection = null;
            bool isapproved = true;
            try
            {
                TargetEntity = (EntityReference)context.InputParameters["Target"] as EntityReference;
                tracingService.Trace("Inside Try Block");
                RelatedEntityCollection = (EntityReferenceCollection)context.InputParameters["RelatedEntities"] as EntityReferenceCollection;

                Entity Contactrecord = service.Retrieve("contact", RelatedEntityCollection[0].Id, new ColumnSet("tt1_contactstatus"));
                OptionSetValue Statusvalue = Contactrecord.GetAttributeValue<OptionSetValue>("tt1_contactstatus");
                tracingService.Trace("Status value is: ", Statusvalue.Value);
                if (Statusvalue != null)
                {
                    //int optionSetValue = optionSet.Value;
                    // Now you have the integer value of the option set, you can use it as needed
                    if (Statusvalue.Value != 958510001)
                    {
                        tracingService.Trace("Status value is: ", Statusvalue.Value);
                        isapproved = false;
                    }
                }
                if (context.MessageName.ToLower() == "associate")
                {
                    //validate relationship
                    if (context.InputParameters.Contains("Relationship"))
                    {
                        relationship = context.InputParameters["Relationship"].ToString();
                        if (relationship != "contact_customer_accounts.Referenced")
                        {
                            return;
                        }
                    }
                    if (!isapproved)
                    {
                        tracingService.Trace("IsApproved is : ", isapproved);
                        //send email code
                        //throw error to prevent association
//                        Entity email = new Entity("email");

//                        Entity fromparty = new Entity("activityparty");
//                        Entity toparty = new Entity("activityparty");

//                        fromparty["partyid"] = new EntityReference("systemuser", context.UserId);
//                        toparty["partyid"] = new EntityReference("contact", context.UserId);

//                        email["from"] = new Entity[] { fromparty };
//                        email["to"] = new Entity[] { toparty };
//                        email["subject"] = "Welcome ";
//                        email["description"] = "Hi ";
//                        email["directioncode"] = true;
//                        email["regardingobjectid"] = new EntityReference("contact", Contactrecord.Id);
//                        Guid emailId = service.Create(email)
//;
//                        tracingService.Trace("Emailid is : ", emailId);
//                        //send email to the recipient
//                        SendEmailRequest emailRequest = new SendEmailRequest
//                        {
//                            EmailId = emailId,
//                            TrackingToken = "",
//                            IssueSend = true
//                        };
//                        //getting email response
//                        SendEmailResponse emailResponse = (SendEmailResponse)service.Execute(emailRequest);
                        tracingService.Trace("Before throw exception");

                        throw new InvalidPluginExecutionException("Contact Status needs to be Approved !");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}

