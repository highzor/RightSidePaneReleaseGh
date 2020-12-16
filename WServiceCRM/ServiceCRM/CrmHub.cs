using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using ServiceCRM.Helpers;

namespace ServiceCRM
{
    public class CrmHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string name, string message)
        {
            string connectionId = Context.ConnectionId;
            Clients.All.addNewMessageToPage(name, message);
        }

        public void SignIn(string inputNumber)
        {
            CrmHelper crm = new CrmHelper();

            Guid connectionId = new Guid(Context.ConnectionId);
            string result = crm.LogIn(inputNumber, connectionId);
            if (result.Equals("all right"))
            {
                Clients.Client(Context.ConnectionId).SignIn();
                //Client(Context.ConnectionId)
            }
            else
            {
                Clients.Client(Context.ConnectionId).addErrorMessage(result);
            }

        }

        public override Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            // Store this connectionId in list -- This will be helpful for tracking list of connected clients.
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            // Remove this connectionId from list
            // and save the message for disconnected clients.
            // Maintain list of disconnected clients in a list, say ABC
            return base.OnDisconnected(stopCalled);
        }
    }
}