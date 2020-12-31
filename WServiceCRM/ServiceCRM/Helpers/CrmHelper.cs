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
        public ResponseHelper LogIn(string inputNumber, Guid connectionId = new Guid())
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "systemuser", "new_shortnumber", inputNumber).Entities.FirstOrDefault();
                if (entity != null)
                {
                    if ((bool)entity.Attributes["new_authenticated"] == false)
                    {
                        entity.Attributes["new_authenticated"] = true;
                        service.Update(entity);
                        response.Code = 200;
                        return response;
                    }
                    else
                    {
                        response.Code = 200;
                        return response;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Code = 404;
                    response.ErrorMessage = "Not Found";
                    return response;
                }
            }
            catch (Exception e)
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
        }

        public ResponseHelper LogOff(string inputNumber, Guid connectionId = new Guid())
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "systemuser", "new_shortnumber", inputNumber).Entities.FirstOrDefault();
                if (entity != null)
                {
                    if ((bool)entity.Attributes["new_authenticated"] == true)
                    {
                        entity.Attributes["new_authenticated"] = false;
                        service.Update(entity);
                        response.Code = 200;
                        return response;
                    }
                    else
                    {
                        response.Code = 200;
                        return response;
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Code = 404;
                    response.ErrorMessage = "Not Found";
                    return response;
                }
            }
            catch (Exception e) 
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
        }
        public CallerHepler IncomingCall(string callId, DateTime callDate, string caller)
        {
            ResponseHelper response = new ResponseHelper();
            CallerHepler callerEntity = null;
            try
            {
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service, "contact", "telephone1", caller);
                CreateActivityEntity(service, callId, callDate, caller, entites);
                callerEntity = GetCaller(entites);
                return callerEntity;
            }
            catch (Exception e)
            {
                callerEntity.Code = 500;
                callerEntity.ErrorMessage = e.Message;
                return callerEntity;
            }
        }

        public ResponseHelper CompleteCall(string callId, DateTime completeDate, string reason)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                if (entity != null)
                {
                    entity["actualend"] = completeDate;
                    entity["statecode"] = new OptionSetValue(1);
                    service.Update(entity);
                    response.Code = 200;
                    return response;
                }
                else
                {
                    response.IsError = true;
                    response.Code = 404;
                    response.ErrorMessage = "Not Found";
                    return response;
                }
            }
            catch (Exception e) 
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
        }
        public string Summary(string callId)
        {
            ResponseHelper response = new ResponseHelper();
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

        public ResponseHelper Answer(string callId)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                if (entity != null)
                {
                    entity["new_answerdate"] = DateTime.Now;
                    service.Update(entity);
                    response.Code = 200;
                    return response;
                }
                else
                {
                    response.IsError = true;
                    response.Code = 404;
                    response.ErrorMessage = "Not Found";
                    return response;
                }
            }
            catch (Exception e)
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
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
            clientCredentials.UserName.UserName = "apetukhov";
            clientCredentials.UserName.Password = "12Qwerty";
            service = (IOrganizationService)new OrganizationServiceProxy(new Uri("http://10.40.10.146/LearnAPetukhov/XRMServices/2011/Organization.svc"),
             null, clientCredentials, null);
            return service;
        }
        private CallerHepler GetCaller(EntityCollection entites)
        {
            Entity entity = entites.Entities.FirstOrDefault();
            CallerHepler callerEntity = new CallerHepler();
            callerEntity.FullName = entity.Attributes["fullname"].ToString();
            try
            {
                callerEntity.DateOfBirth = DateTime.Parse(entity.Attributes["birthdate"].ToString()).Date.ToString("d");
            } catch { callerEntity.DateOfBirth = null; }
            callerEntity.PhoneOfCaller = entity.Attributes["telephone1"].ToString();
            callerEntity.Code = 200;
            return callerEntity;
        }

    }
}