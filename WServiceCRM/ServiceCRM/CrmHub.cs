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

        public string SignIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            string result = crm.LogIn(inputNumber, connectionId);
            if (result.Equals("200"))
            {
                SignalRUser user = new SignalRUser();
                user.ConnectionId = connectionId;
                user.ShortNumber = inputNumber;
                connectionsList.Add(user);
            }
            return result;
        }

        public string SignOut(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();
            Guid connectionId = new Guid(Context.ConnectionId);
            string result = crm.LogOff(inputNumber, connectionId);
            if (result.Equals("200"))
            {
               SignalRUser user = connectionsList.Find(x => x.ConnectionId == connectionId);
                connectionsList.Remove(user);
            }
            return result;
        }

        public string IncomingCall(string callId, DateTime date, string phoneOfCaller, string fullNameOfCaller, string dateOfBirthOfCaller, string shortNumber)
        {
            try
            {
                SignalRUser user = connectionsList.Find(x => x.ShortNumber.Equals(shortNumber));
                var context = GlobalHost.ConnectionManager.GetHubContext<CrmHub>();
                context.Clients.Client(user.ConnectionId.ToString()).IncomingCall(callId, date, phoneOfCaller, fullNameOfCaller, dateOfBirthOfCaller);
            } catch (Exception e) { return e.Message; }
            return "200";
        }

        public string CompleteCall(string callId, DateTime completeDate, string reason)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.CompleteCall(callId, completeDate, reason);
            return result;
        }
        public string Summary(string callId)
        {
            return "";
        }
        public string Answer(string callId)
        {
            CrmHelper crm = new CrmHelper();
            string result = crm.Answer(callId);
            return result;
        }
        public string Deny(string callId)
        {
            return "200";
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
