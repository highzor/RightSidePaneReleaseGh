
using System;

namespace ServiceCRM.Helpers
{
    public class CallerHepler
    {
        public int? Code { get; set; }
        public string ErrorMessage { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneOfCaller { get; set; }
        public string CallId { get; set; }
        public DateTime? DateOfCall { get; set; }
        public string UserShortNumber { get; set; }
        public Guid PhoneCallId { get; set; }
        public Guid ContactId { get; set; }
    }
}