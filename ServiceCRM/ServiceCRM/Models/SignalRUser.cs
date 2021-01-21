using System;
using System.Collections.Generic;

namespace ServiceCRM.Models
{
    public class SignalRUser
    {
        public string UserName { get; set; }
        public List<Guid> ConnectionId { get; set; }
        public string ShortNumber { get; set; }
    }
}