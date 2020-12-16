using System;

namespace ServiceCRM.Models
{
    public class SignalRUser
    {
        public string UserName { get; set; }
        public Guid ConnectionId { get; set; }
    }
}