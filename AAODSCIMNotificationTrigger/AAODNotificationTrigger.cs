using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using Kaseya.AuthAnvil.Models;
using NotifcationTrigger.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace AAODSCIMNotificationTrigger
{
    public static class AAODNotificationTrigger
    {
        [FunctionName("AAODNotificationTrigger")]
        public static void Run([ServiceBusTrigger("sb-q-change-notifications", AccessRights.Listen, Connection = "TargetServiceBusQueue")]EntityChange entityChangeItem , TraceWriter log)
        {
         //   throw new Exception("test");

            string test = entityChangeItem.Type;
            //Person or Group
            if (test == EntityTypes.Person.ToString() || test == EntityTypes.Group.ToString())
            {
                //process this message
                TestFilter(entityChangeItem, log);
                log.Info($"C# ServiceBus queue trigger function processed message: {entityChangeItem.EntityId.ToString()}");
            }
            else
            {
                //don't care about the message
                log.Info($"Did not process message type: {entityChangeItem.Type}");
            }

        }

        private static void TestFilter(EntityChange entityChangeItem, TraceWriter log)
        {
            IEnumerable<string> changed = null;
            string changes = string.Empty;
            try
            {
                if (TenantConfiguration.IsValidSCIMClient(entityChangeItem.TenantId))
                {

                    #region this is for testing purposes only

                    Dictionary<string, IEnumerable<string>> dic = new Dictionary<string, IEnumerable<string>>();
                    if (entityChangeItem.ChangedProperties.Count > 0)
                    {
                        dic = entityChangeItem.ChangedProperties;
                        foreach (string key in dic.Keys)
                        {
                            if (dic.TryGetValue(key, out changed))
                            {
                                changes = string.Join(" ", changed);
                                changes = string.Format("Changed key value : {0} Changed string: {1}", key, changes);
                            }
                            else
                            {
                                changes = "No changes found";
                            }
                            break;
                        }


                    }
                    //this needs to be passed along
                    log.Info($"Found client to process: {entityChangeItem.TenantId.ToString()} Changes: {changes}");

                 #endregion

                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.Message}");

            }
        }
       
    }
}
