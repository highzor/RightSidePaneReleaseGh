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
        /// <summary>
        /// Получение сущности 'systemuser' и заполнение поля на карточке пользователя 'Пользователь атворизован' = Да. 
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
        public ResponseHelper LogIn(string inputNumber)
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
        /// <summary>
        /// Получение сущности 'systemuser' и заполнение поля на карточке пользователя 'Пользователь атворизован' = Нет.
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
        public ResponseHelper LogOff(string inputNumber)
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
        /// <summary>
        /// Получение сущностей 'contact', 'systemuser', вызов метода cоздания сущности 'Звонок', конструктор для 'contact'
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <param name="callDate">Дата события звонка</param>
        /// <param name="caller">№Телефона вызывающего абонента от АТС</param>
        /// <param name="userShortNumber">Короткий номер пользователя, который вызывают от АТС</param>
        /// <returns>сущность 'contact', вызывающего абонента</returns>
        public CallerHepler CreatePhoneCall(string callId, DateTime callDate, string caller, string userShortNumber)
        {
            ResponseHelper response = new ResponseHelper();
            CallerHepler callerEntity = null;
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity CallerEntites = GetEntities(service, "contact", "telephone1", caller).Entities.FirstOrDefault();
                Entity SystemUserEntites = GetEntities(service, "systemuser", "new_shortnumber", userShortNumber).Entities.FirstOrDefault();
                Guid phoneCallId = CreateActivityEntityPhoneCall(service, callId, callDate, caller, CallerEntites, SystemUserEntites);
                callerEntity = GetCaller(CallerEntites, callId, callDate, userShortNumber, phoneCallId);
                return callerEntity;
            }
            catch (Exception e)
            {
                callerEntity.Code = 500;
                callerEntity.ErrorMessage = e.Message;
                return callerEntity;
            }
        }
        /// <summary>
        /// Проставление полей в сущности 'Звонок' при завершении звонка
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <param name="completeDate">Дата завершения звонка</param>
        /// <param name="reason">Описание(причина) завершения</param>
        /// <returns>status code</returns>
        public ResponseHelper SetAttrsCompleteCall(string callId, DateTime completeDate, string reason)
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
                    entity["description"] = reason;
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
        /// <summary>
        /// Получения полей сущности 'Звонок'
        /// </summary>
        /// <param name="callId">ID звонка, который создан</param>
        /// <returns>поля сущности Key-Value в 'string'</returns>
        public string GetSummaryFields(string callId)
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
        /// <summary>
        /// Проставление полей в сущности 'Звонок' при принятии вызова
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <returns>status code</returns>
        public ResponseHelper SetAttrsAnswer(string callId)
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
        /// <summary>
        /// Получение сущности 'Звонок', вызов метода cоздания сущности 'Обращение'
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <returns>status code</returns>
        public ResponseHelper CreateIncident(string callId)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                IOrganizationService service = ConnectToCRM();
                Entity entity = GetEntities(service, "phonecall", "new_callid", callId).Entities.FirstOrDefault();
                Guid incidentId = CreateActivityEntityIncident(service, entity);
                response.TransferParam = incidentId;
                response.Code = 200;
            }
            catch (Exception e)
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
            return response;
        }
        /// <summary>
        /// Создание сущности 'Обращение'
        /// </summary>
        /// <param name="service">системный параметр</param>
        /// <param name="entity">сущность 'phonecall'</param>
        /// <returns>ID сущности 'Обращение'</returns>
        private Guid CreateActivityEntityIncident(IOrganizationService service, Entity entity)
        {
            Entity incidentEntity = new Entity("incident");
            EntityCollection From = (EntityCollection)entity.Attributes["from"];
            Entity PartyEntity = From.Entities.FirstOrDefault();
            EntityReference contact = (EntityReference)PartyEntity.Attributes["partyid"];
            EntityReference owner = (EntityReference)PartyEntity.Attributes["ownerid"];
            EntityReference CustomerId = new EntityReference("contact", contact.Id);
            EntityReference OwnerId = new EntityReference("systemuser", owner.Id);
            incidentEntity["customerid"] = CustomerId;
            incidentEntity["ownerid"] = OwnerId;
            Guid incidentId = service.Create(incidentEntity);
            return incidentId;
        }
        /// <summary>
        /// Создание сущности 'Звонок'
        /// </summary>
        /// <param name="service">системный параметр</param>
        /// <param name="callId">ID звонка</param>
        /// <param name="callDate">время начала события 'Звонок'</param>
        /// <param name="caller">№Телефона вызывающего абонента от АТС</param>
        /// <param name="callerEntity">Сущность 'contact'</param>
        /// <param name="systemUserEntity">Сущность 'systemuser'</param>
        /// <returns>ID сущности 'Звонок'</returns>
        private Guid CreateActivityEntityPhoneCall(IOrganizationService service, string callId, DateTime callDate, string caller, Entity callerEntity, Entity systemUserEntity)
        {
            Entity toUser = new Entity("activityparty");
            Entity from = new Entity("activityparty");
            Entity phoneCallEntity = new Entity("phonecall");
            EntityReference OwnerUser = new EntityReference("systemuser", systemUserEntity.Id);
            toUser["partyid"] = OwnerUser;
            from["partyid"] = new EntityReference("contact", callerEntity.Id);
            phoneCallEntity["from"] = new Entity[] { from };
            phoneCallEntity["to"] = new Entity[] { toUser };
            phoneCallEntity["ownerid"] = OwnerUser;
            phoneCallEntity["new_callid"] = callId;
            phoneCallEntity["new_calldate"] = callDate;
            phoneCallEntity["phonenumber"] = caller;
            Guid phoneCallId = service.Create(phoneCallEntity);
            return phoneCallId;
        }
        /// <summary>
        /// Получение сущностей (коллекция)
        /// </summary>
        /// <param name="service">системный параметр</param>
        /// <param name="entity">имя сущности, которую требуется получить</param>
        /// <param name="attribute">фильтрующий атрибут</param>
        /// <param name="value">значение фильтра</param>
        /// <returns>коллекция сущностей</returns>
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
        /// <summary>
        /// Создание объекта фильтрации
        /// </summary>
        /// <param name="attribute">фильтрующий атрибут</param>
        /// <param name="value">значение фильтра</param>
        /// <returns>объекта фильтрации</returns>
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
        /// <summary>
        /// Соединение приложения C# с CRM
        /// </summary>
        /// <returns>объект соединения</returns>
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
        /// <summary>
        /// Конструктор 'contact' для отправки объекта с полями информации о вызывающем абоненте АТС
        /// </summary>
        /// <param name="entity">сущность 'contact'</param>
        /// <param name="callId">ID звонка</param>
        /// <param name="callDate">Дата события звонка</param>
        /// <param name="userShortNumber">Короткий номер пользователя</param>
        /// <returns>объект с полями информации о вызывающем абоненте АТС</returns>
        private CallerHepler GetCaller(Entity entity, string callId, DateTime callDate, string userShortNumber, Guid phoneCallId)
        {
            CallerHepler callerEntity = new CallerHepler();
            callerEntity.UserShortNumber = userShortNumber;
            callerEntity.CallId = callId;
            callerEntity.DateOfCall = callDate;
            callerEntity.PhoneCallId = phoneCallId;
            callerEntity.ContactId = entity.Id;
            try
            {
                callerEntity.FullName = entity.Attributes["fullname"].ToString();
            } catch { callerEntity.FullName = null; }
            try
            {
                callerEntity.DateOfBirth = DateTime.Parse(entity.Attributes["birthdate"].ToString()).Date.ToString("d");
            } catch { callerEntity.DateOfBirth = null; }
            try
            {
                callerEntity.PhoneOfCaller = entity.Attributes["telephone1"].ToString();
            } catch { callerEntity.PhoneOfCaller = null; }
            callerEntity.Code = 200;
            return callerEntity;
        }

    }
}
