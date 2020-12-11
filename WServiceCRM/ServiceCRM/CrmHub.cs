﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using ServiceCRM.Models;

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