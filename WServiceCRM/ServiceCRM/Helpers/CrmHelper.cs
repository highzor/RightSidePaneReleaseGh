using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel.Description;
using Newtonsoft.Json;

namespace ServiceCRM.Helpers
{
    public class CrmHelper
    {
        public string LogIn(string inputNumber, Guid connectionId = new Guid())
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "systemuser", "new_shortnumber", inputNumber).Entities.FirstOrDefault();
                if (entity != null)
                {
                    if (entity.Attributes["new_authenticated"].ToString().Equals("False"))
                    {
                        SetAuth(entity, true);
                        service.Update(entity);
                        return "200";
                    }
                    else
                    {
                        return "200";
                    }
                }
                else
                {
                    return "404";
                }
            }
            catch (Exception e) { return e.Message; }
        }

        public string LogOff(string inputNumber, Guid connectionId = new Guid())
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "systemuser", "new_shortnumber", inputNumber).Entities.FirstOrDefault();
                if (entity != null)
                {
                    if (entity.Attributes["new_authenticated"].ToString().Equals("True"))
                    {
                        SetAuth(entity, false);
                        service.Update(entity);
                        return "200";
                    }
                    else
                    {
                        return "200";
                    }
                }
                else
                {
                    return "404";
                }
            }
            catch (Exception e) { return e.Message; }
        }
        public string IncommingCall(string callId, DateTime callDate, string caller)
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, "contact", "telephone1", caller);
                CreateActivityEntity(service, callId, callDate, caller, entites);
                return "200";
            }
            catch (Exception e) { return e.Message; }
        }

        public string CompleteCall(string callId, DateTime completeDate, string reason)
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                entity["actualend"] = completeDate;
                entity["statecode"] = new OptionSetValue(1);
                service.Update(entity);
                return "200";
            }
            catch (Exception e) { return e.Message; }
        }
        public string Summary(string callId)
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                if (entity != null)
                {
                    string serialize = JsonConvert.SerializeObject(entity.Attributes);
                    string result = serialize.Substring(1, serialize.Length - 2);
                    return result;
                }
                else
                {
                    return "404";
                }
            }
            catch (Exception e) { return e.Message; }
        }

        public string Answer(string callId)
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                entity["new_answerdate"] = DateTime.Now;
                service.Update(entity);
                return "200";
            }
            catch (Exception e)
            { return e.Message; }
        }
        private void CreateActivityEntity(IOrganizationService service, string callId, DateTime callDate, string caller, EntityCollection entites)
        {
            Entity toUser = new Entity("activityparty");
            Entity from = new Entity("activityparty");
            Entity phoneCallEntity = new Entity("phonecall");
            toUser["partyid"] = new EntityReference("systemuser", new Guid("9344DC04-8804-EB11-B810-005056964201"));
            from["partyid"] = new EntityReference("contact", new Guid(entites.Entities.FirstOrDefault().Attributes["contactid"].ToString()));
            phoneCallEntity["from"] = new Entity[] { from };
            phoneCallEntity["to"] = new Entity[] { toUser };
            phoneCallEntity["new_callid"] = callId;
            phoneCallEntity["new_calldate"] = callDate;
            phoneCallEntity["phonenumber"] = caller;
            service.Create(phoneCallEntity);
        }
        private EntityCollection GetEntities(IOrganizationService service, string entity, string attribute = "", string value = "")
        {
            EntityCollection collection = null;
            QueryExpression query = new QueryExpression(entity);
            if (!String.IsNullOrEmpty(attribute))
            {
                FilterExpression filter = GetFilter(attribute, value);
                query.Criteria.AddFilter(filter);
            }
            query.ColumnSet = new ColumnSet(true);
            collection = service.RetrieveMultiple(query);
            return collection;
        }
        private FilterExpression GetFilter(string attribute, string value)
        {
            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = attribute;
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(value);
            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);
            return filter1;
        }
        private IOrganizationService ConnectToCRM()
        {
            IOrganizationService service = null;
            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "user";
            clientCredentials.UserName.Password = "password";
            service = (IOrganizationService)new OrganizationServiceProxy(new Uri("http://XX.XX.XX.XXX/LearnAPetukhov/XRMServices/2011/Organization.svc"),
             null, clientCredentials, null);
            return service;
        }
        private void SetAuth(Entity entity, bool option)
        {
            entity.Attributes["new_authenticated"] = option;
        }

    }
}
