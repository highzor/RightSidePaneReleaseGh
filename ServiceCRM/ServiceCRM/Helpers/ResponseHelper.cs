
using System;

namespace ServiceCRM.Helpers
{
    public class ResponseHelper
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public int Code { get; set; }
        public Guid TransferParam { get; set; }
    }
}