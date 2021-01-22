using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using ServiceCRM.Helpers;
using ServiceCRM.Models;

namespace ServiceCRM
{
    public class CrmHub : Hub
    {
        public static List<SignalRUser> connectionsList = new List<SignalRUser>();
        /// <summary>
        /// Метод авторизации extension SignalR (вызывается из браузера)
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
        public ResponseHelper SignIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            ResponseHelper response = crm.LogIn(inputNumber);
            if (!response.IsError)
            {
                SignalRUser user = new SignalRUser();
                user.ConnectionId = new List<Guid> { connectionId };
                user.ShortNumber = inputNumber;
                connectionsList.Add(user);
            }
            return response;
        }
        /// <summary>
        /// Выход из extension
        /// </summary>
        /// <param name="inputNumber">Короткий номер пользователя</param>
        /// <returns>status code</returns>
        public ResponseHelper SignOut(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            ResponseHelper response = crm.LogOff(inputNumber);
            if (!response.IsError)
            {
               SignalRUser user = connectionsList.Find(x => x.ShortNumber.Equals(inputNumber));
                connectionsList.Remove(user);
            }
            return response; 
        }
        /// <summary>
        /// Запуск звонка в браузере
        /// </summary>
        /// <param name="callerHelper">Объект с полями информации о вызывающем абоненте АТС + Короткий номер пользователя + ID звонка</param>
        /// <returns>status code</returns>
        public ResponseHelper ExecuteSignalRIncomingCall(CallerHepler callerHelper)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                SignalRUser user = connectionsList.Find(x => x.ShortNumber.Equals(callerHelper.UserShortNumber));
                Guid[] connectionsIds = user.ConnectionId.ToArray();
                string[] connectionsIdsToString = Array.ConvertAll(connectionsIds, x => x.ToString());
                var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();
                context.Clients.Clients(connectionsIdsToString).IncomingCall(callerHelper);
                response.Code = 200;
            } catch (Exception e) 
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
            return response;
        }
        /// <summary>
        /// Запуск завершения звонка из браузера
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <param name="completeDate">Дата завершения звонка</param>
        /// <param name="reason">Описание(причина) завершения</param>
        /// <returns>status code</returns>
        public ResponseHelper CompleteCall(string callId, DateTime completeDate, string reason)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                Guid connectionId = new Guid(Context.ConnectionId);
                SignalRUser user = connectionsList.Find(x => x.ConnectionId.Contains(connectionId));
                CrmHelper crm = new CrmHelper();
                response = crm.SetAttrsCompleteCall(callId, completeDate, reason);
                Guid[] connectionsIds = user.ConnectionId.ToArray();
                string[] connectionsIdsToString = Array.ConvertAll(connectionsIds, x => x.ToString());
                var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();
                context.Clients.Clients(connectionsIdsToString).BackToPage();
            } catch (Exception e)
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
            return response;
        }
        public string Summary(string callId)
        {
            return "";
        }
        /// <summary>
        /// Запуск ответа на звонок из браузера
        /// </summary>
        /// <param name="callId">ID звонка</param>
        /// <returns>status code</returns>
        public ResponseHelper Answer(string callId)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                Guid connectionId = new Guid(Context.ConnectionId);
                SignalRUser user = connectionsList.Find(x => x.ConnectionId.Contains(connectionId));
                CrmHelper crm = new CrmHelper();
                response = crm.SetAttrsAnswer(callId);
                if (!response.IsError)
                {
                    response = crm.CreateIncident(callId);
                    Guid[] connectionsIds = user.ConnectionId.ToArray();
                    string[] connectionsIdsToString = Array.ConvertAll(connectionsIds, x => x.ToString());
                    var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();
                    context.Clients.Clients(connectionsIdsToString).SuccessAnswer(response.TransferParam);
                }
            } catch (Exception e)
            {
                response.IsError = true;
                response.ErrorMessage = e.Message;
                response.Code = 500;
                return response;
            }
            return response;
        }
        /// <summary>
        /// Триггер SignalR на присоединение пользователя CRM
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string shortNumber = Context.QueryString["shortNumber"];
            if (shortNumber != null)
            {
                //SignalRUser user = new SignalRUser();
                Guid connectionId = new Guid(Context.ConnectionId);
                SignalRUser user = connectionsList.Find(x => x.ShortNumber.Equals(shortNumber));
                if (user != null)
                {
                    user.ConnectionId.Add(connectionId);
                } else
                {
                    user = new SignalRUser();
                    user.ShortNumber = shortNumber;
                    user.ConnectionId = new List<Guid> { connectionId };
                    connectionsList.Add(user);
                }
                //user.ShortNumber = shortNumber;
                
            }
            return base.OnConnected();
        }
        /// <summary>
        /// Триггер SignalR на переподключение пользователя CRM
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            Guid connectionId = new Guid(Context.ConnectionId);

            return base.OnReconnected();
        }
        /// <summary>
        /// Триггер SignalR на отсоединение пользователя CRM
        /// </summary>
        /// <param name="stopCalled">системный параметр</param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            Guid connectionId = new Guid(Context.ConnectionId);
            try
            {
                SignalRUser user = connectionsList.Find(x => x.ConnectionId.Contains(connectionId));
                if (user != null && user.ConnectionId.Count > 1)
                {
                    Guid id = user.ConnectionId.Find(x => x == connectionId);
                    user.ConnectionId.Remove(id);
                } else if (user != null)
                {
                    connectionsList.Remove(user);
                }
            } catch { }
            return base.OnDisconnected(stopCalled);
        }
    }
}