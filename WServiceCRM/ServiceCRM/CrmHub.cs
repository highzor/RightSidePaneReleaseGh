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
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string name, string message)
        {
            string connectionId = Context.ConnectionId;
            Clients.All.addNewMessageToPage(name, message);
        }

        public ResponseHelper SignIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            ResponseHelper response = crm.LogIn(inputNumber, connectionId);
            if (!response.IsError)
            {
                SignalRUser user = new SignalRUser();
                user.ConnectionId = connectionId;
                user.ShortNumber = inputNumber;
                connectionsList.Add(user);
            }
            return response;
        }

        public ResponseHelper SignOut(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            ResponseHelper response = crm.LogOff(inputNumber, connectionId);
            if (!response.IsError)
            {
               SignalRUser user = connectionsList.Find(x => x.ConnectionId == connectionId);
                connectionsList.Remove(user);
            }
            return response; 
        }

        public ResponseHelper IncomingCall(string callId, DateTime date, string phoneOfCaller, string fullNameOfCaller, string dateOfBirthOfCaller)
        {
            ResponseHelper response = new ResponseHelper();
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();
                context.Clients.All.IncomingCall(callId, date, phoneOfCaller, fullNameOfCaller, dateOfBirthOfCaller);
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

        public ResponseHelper CompleteCall(string callId, DateTime completeDate, string reason)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.CompleteCall(callId, completeDate, reason);
            return response;
        }
        public string Summary(string callId)
        {
            return "";
        }
        public ResponseHelper Answer(string callId)
        {
            CrmHelper crm = new CrmHelper();
            ResponseHelper response = crm.Answer(callId);
            return response;
        }
        public ResponseHelper Deny(string callId)
        {
            ResponseHelper response = new ResponseHelper();
            response.Code = 200;
            return response;
        }
        public override Task OnConnected()
        {
            string shortNumber = Context.QueryString["shortNumber"];
            if (shortNumber != null)
            {
                SignalRUser user = new SignalRUser();
                Guid connectionId = new Guid(Context.ConnectionId);
                user.ConnectionId = connectionId;
                user.ShortNumber = shortNumber;
                connectionsList.Add(user);
            }
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            Guid connectionId = new Guid(Context.ConnectionId);

            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var shortNumber = Clients.CallerState.shortNumber;
            Guid connectionId = new Guid(Context.ConnectionId);
            try
            {
                SignalRUser user = connectionsList.Find(x => x.ConnectionId == connectionId);
                connectionsList.Remove(user);
            } catch { }
            return base.OnDisconnected(stopCalled);
        }
    }
}
