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
            }
            else
            {
                Clients.Client(Context.ConnectionId).addErrorMessage(result);
            }

        }

        public override Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
    }
}
