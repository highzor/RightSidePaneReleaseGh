using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ServiceCRM.Models
{
    public class CrmHelper : IDisposable
    {
        private Dictionary<string, dynamic> attributesValues = new Dictionary<string, dynamic>();
        private string callMethod;
        public void Dispose()
        {

        }
        public string StartsWork(string inputNumber)
        {
            try
            {
                callMethod = new StackTrace(false).GetFrame(0).GetMethod().Name;
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, false, "contact");
                string result = SetAuth(entites, inputNumber, service);
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string StopsWork(string shortNumber)
        {
            try
            {
                callMethod = new StackTrace(false).GetFrame(0).GetMethod().Name;
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, false, "contact");
                string result = SetAuth(entites, shortNumber, service);
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string IncommingCall(string callId, DateTime callDate, string caller)
        {
            try
            {
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, true, "contact", "telephone1", caller);
                CreateActivityEntity(service, callId, callDate, caller, entites);
                return "Получен входящий вызов";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string CompleteCall(string callId, DateTime completeDate, string reason)
        {
            try
            {
                attributesValues.Add("actualend", completeDate);
                attributesValues.Add("statecode", new OptionSetValue(1));
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, true, "phonecall", "new_callid", callId);
                entites = SetAttributesDict<EntityCollection>(entites);
                service.Update(entites.Entities.First());
                return "Звонок завершен";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string Summary(string callId)
        {
            IOrganizationService service = ConnectToCRM();
            EntityCollection entites = GetEntities(service, true, "phonecall", "new_callid", callId);
            string tst = entites.Entities.First().Attributes.Keys.ToString();

            return "";
        }

        public string Answer(string callId)
        {
            try
            {
                attributesValues.Add("new_answerdate", DateTime.Now);
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, true, "phonecall", "new_callid", callId);
                entites = SetAttributesDict<EntityCollection>(entites);
                service.Update(entites.Entities.First());
                return "Вызов принят";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private void CreateActivityEntity(IOrganizationService service, string callId, DateTime callDate, string caller, EntityCollection entites)
        {
            Entity From = new Entity("activityparty");
            attributesValues.Add("partyid", new EntityReference("systemuser", new Guid("9344DC04-8804-EB11-B810-005056964201")));
            From = SetAttributesDict<Entity>(null, From);
            //From["partyid"] = new EntityReference("systemuser", new Guid("9344DC04-8804-EB11-B810-005056964201")); // Need to replace with Guid
            Entity phoneCallEntity = new Entity("phonecall");
            attributesValues.Add("subject", "Звонок: " + callId);
            attributesValues.Add("description", "Звонок с абонентом: " + caller);
            attributesValues.Add("new_callid", callId);
            attributesValues.Add("new_calldate", callDate);
            attributesValues.Add("phonenumber", caller);
            attributesValues.Add("from", new Entity[] { From });
            phoneCallEntity = SetAttributesDict<Entity>(null, phoneCallEntity);
            //phoneCallEntity["from"] = new Entity[] { From };
            Entity[] entityArray = new Entity[entites.Entities.Count];
            for (int i = 0; i < entites.Entities.Count; i++)
            {
                attributesValues.Add("partyid", new EntityReference("contact", new Guid(entites.Entities[i].Attributes["contactid"].ToString())));
                Entity toContact = new Entity("activityparty");
                toContact = SetAttributesDict<Entity>(null, toContact);
                // toContact["partyid"] = new EntityReference("contact", new Guid(entites.Entities[i].Attributes["contactid"].ToString()));
                entityArray[i] = toContact;
            }
            attributesValues.Add("to", entityArray);
            phoneCallEntity = SetAttributesDict<Entity>(null, phoneCallEntity);
            Guid phonecallId = service.Create(phoneCallEntity);
        }
        private EntityCollection GetEntities(IOrganizationService service, bool needFilter, string entity, string attribute = "", string value = "")
        {
            EntityCollection collection = null;
            QueryExpression query = new QueryExpression(entity);
            if (needFilter == true)
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
            clientCredentials.UserName.UserName = "username";
            clientCredentials.UserName.Password = "password";
            service = (IOrganizationService)new OrganizationServiceProxy(new Uri("http://XX.XX.XX.XXX/LearnAPetukhov/XRMServices/2011/Organization.svc"),
             null, clientCredentials, null);
            return service;
        }
        private string SetAuth(EntityCollection entites, string inputNumber, IOrganizationService service)
        {
            string result = "all right";

            foreach (Entity itemEntity in entites.Entities)
            {
                try
                {
                    Guid entityId = new Guid(itemEntity.Attributes["contactid"].ToString());
                    var auth = itemEntity.Attributes["new_authenticated"].ToString();
                    string num = itemEntity.Attributes["new_shortnumber"].ToString();
                    if (num.Equals(inputNumber))
                    {
                        if (auth.Equals("False") && callMethod.Equals("StartsWork"))
                        {
                            itemEntity.Attributes["new_authenticated"] = true;
                        }
                        else if (auth.Equals("True") && callMethod.Equals("StopsWork"))
                        {
                            itemEntity.Attributes["new_authenticated"] = false;
                        }
                        service.Update(itemEntity);
                        return result;
                    }
                }
                catch { }
            }
            return "no matches for the entered number";
        }
<<<<<<< HEAD
        private T SetAttributesDict<T>(EntityCollection entites = null, Entity entity = null)
        {
            if (entites != null)
            {
                foreach (Entity itemEntity in entites.Entities)
                {
                    foreach (KeyValuePair<string, dynamic> pair in attributesValues)
                    {
                        try
                        {
                            itemEntity.Attributes[pair.Key] = pair.Value;
                        }
                        catch { }
                    }
                }
                attributesValues.Clear();
                object objEntities = entites;
                return (T)Convert.ChangeType(objEntities, typeof(T));
            }
            else
            {
                foreach (KeyValuePair<string, dynamic> pair in attributesValues)
                {
                    try
                    {
                        entity.Attributes[pair.Key] = pair.Value;
                    }
                    catch { }
                }
                attributesValues.Clear();
                object objEntities = entity;
                return (T)Convert.ChangeType(objEntities, typeof(T));
            }
        }
=======
>>>>>>> 3f1ef927c5f2874b89f474aa4915b592fb660026
    }
}
