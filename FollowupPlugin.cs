using Microsoft.Xrm.Sdk;
using System;

namespace myFirstPlugin
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class FollowupPlugin : PluginBase
    {
        public FollowupPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(FollowupPlugin))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;

            // Create a task activity to follow up with the account customer in 7 days.
            Entity followup = new Entity("task");

            followup["subject"] = "Send e-mail to the new customer.";
            followup["description"] =
                "Follow up with the customer. Check if there are any new issues that need resolution.";
            followup["scheduledstart"] = DateTime.Now.AddDays(7);
            followup["scheduledend"] = DateTime.Now.AddDays(7);
            followup["category"] = context.PrimaryEntityName;

            // Refer to the account in the task activity.
            if (context.OutputParameters.Contains("id"))
            {
                Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                string regardingobjectidType = "account";

                followup["regardingobjectid"] = new EntityReference(
                    regardingobjectidType,
                    regardingobjectid
                );
            }

            // Create the task in Microsoft Dynamics CRM.
            localPluginContext.PluginUserService.Create(followup);
        }
    }
}
